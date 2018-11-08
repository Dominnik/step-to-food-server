using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StepToFoodServer.Database;
using StepToFoodServer.Extensions;
using StepToFoodServer.Models;
using StepToFoodServer.Repositories;
using StepToFoodServer.Response;

namespace StepToFoodServer.Controllers
{
    [Route("[controller]")]
    public class FoodController : Controller
    {
        private readonly IBusinessLogicLayer businessLogicLayer;
        private readonly IRepository<Food> foodRepository;

        enum FoodType
        {
            RECOMMENDED = 0,
            ADDED = 1,
            LIKE = 2,
            COMPOSED = 3
        }

        public FoodController(IBusinessLogicLayer businessLogicLayer, IRepository<Food> foodRepository)
        {
            this.businessLogicLayer = businessLogicLayer;
            this.foodRepository = foodRepository;
        }

        [HttpGet]
        public BaseResponse<Food> Get()
        {
            BaseResponse<Food> response = null;
            try
            {
                string token = Request.Headers["Auth"];
                int foodId = int.Parse(Request.Query["foodId"]);
                Food food = businessLogicLayer.FoodWithProducts(token, foodId);
                response = new BaseResponse<Food>(food);
            }
            catch (Exception ex)
            {
                response = new BaseResponse<Food>();
                response.Error = ex.Message;
            }
            return response;
        }

        [HttpPost("add")]
        public BaseResponse<int> Add([FromBody]Food food)
        {
            BaseResponse<int> response = null;
            try
            {
                string token = Request.Headers["Auth"];
                User user = businessLogicLayer.Check(token);

                int foodId = businessLogicLayer.AddFoodWithProducts(user.Id, food);
                response = new BaseResponse<int>(foodId);
            }
            catch (Exception ex)
            {
                response = new BaseResponse<int> { Error = ex.Message };
            }
            return response;
        }

        [HttpGet("remove")]
        public BaseResponse<int> Remove()
        {
            BaseResponse<int> response = null;
            try
            {
                string token = Request.Headers["Auth"];
                int foodId = int.Parse(Request.Query["foodId"]);
                User user = businessLogicLayer.Check(token);
                Food food = foodRepository.Get(foodId);
                if (user.Id != food.Author.Id)
                    throw new MethodAccessException("Removing other people's recipes is not available");

                foodRepository.Delete(foodId);
                response = new BaseResponse<int>(0);
            }
            catch (Exception ex)
            {
                response = new BaseResponse<int> { Error = ex.Message };
            }
            return response;
        }

        [HttpPost("update")]
        public BaseResponse<int> Update([FromBody]Food food)
        {
            BaseResponse<int> response = null;
            try
            {
                string token = Request.Headers["Auth"];
                User user = businessLogicLayer.Check(token);
                if (food.Author.Id != user.Id)
                    throw new MethodAccessException("Updating other people's recipes is not available");

                using (WebClient webClient = new WebClient())
                {
                    byte[] image = webClient.DownloadData(food.Image);
                    food.Image = Convert.ToBase64String(image);
                }
                businessLogicLayer.UpdateFoodWithProducts(food);
                response = new BaseResponse<int>(0);
            }
            catch (Exception ex)
            {
                response = new BaseResponse<int> { Error = ex.Message };
            }
            return response;
        }

        [HttpGet("get/image")]
        public ActionResult GetImage()
        {
            ActionResult response = null;
            try
            {
                int foodId = int.Parse(Request.Query["foodId"]);
                string imageString = foodRepository.Get(foodId).Image;
                byte[] image = Convert.FromBase64String(imageString ?? "");
                response = File(image, "image/jpeg");
            }
            catch
            {
                response = new NoContentResult();
            }
            return response;
        }

        [HttpPost("set/image")]
        public BaseResponse<int> SetImage()
        {
            BaseResponse<int> response = null;
            try
            {
                string token = Request.Headers["Auth"];
                int foodId = int.Parse(Request.Query["foodId"]);
                User user = businessLogicLayer.Check(token);
                Food food = foodRepository.Get(foodId);
                if (user.Id != food.Author.Id)
                    throw new MethodAccessException("Updating other people's recipes is not available");

                IFormFile file = Request.Form.Files[0];
                food.Image = file.ToBase64String();
                foodRepository.Update(food);
                response = new BaseResponse<int>(0);
            }
            catch (Exception ex)
            {
                response = new BaseResponse<int> { Error = ex.Message };
            }
            return response;
        }

        [HttpGet("like")]
        public BaseResponse<int> Like()
        {
            BaseResponse<int> response = null;
            try
            {
                string token = Request.Headers["Auth"];
                User user = businessLogicLayer.Check(token);

                int foodId = int.Parse(Request.Query["foodId"]);
                bool hasLike = bool.Parse(Request.Query["hasLike"]);
                businessLogicLayer.LikeForFood(user.Id, foodId, hasLike);
                response = new BaseResponse<int>(0);
            }
            catch (Exception ex)
            {
                response = new BaseResponse<int> { Error = ex.Message };
            }
            return response;
        }

        [HttpGet("search")]
        public BaseResponse<List<Food>> Search()
        {
            BaseResponse<List<Food>> response = null;
            try
            {
                string token = Request.Headers["Auth"];
                int userId = int.Parse(Request.Query["userId"]);
                int start = int.Parse(Request.Query["start"]);
                int size = int.Parse(Request.Query["size"]);
                string searchName = (string)Request.Query["searchName"] ?? "";
                FoodType foodType = (FoodType)Enum.Parse(typeof(FoodType), Request.Query["foodType"]);

                List<Food> foods = TypeBasedSearchFoods(token, foodType, userId, searchName, start, size);
                response = new BaseResponse<List<Food>>(foods);
            }
            catch (Exception ex)
            {
                response = new BaseResponse<List<Food>> { Error = ex.Message };
            }
            return response;
        }

        [HttpPost("search/products")]
        public BaseResponse<List<Food>> GetByProducts([FromBody]List<int> productIds)
        {
            BaseResponse<List<Food>> response = null;
            try
            {
                string token = Request.Headers["Auth"];
                int start = int.Parse(Request.Query["start"]);
                int size = int.Parse(Request.Query["size"]);

                List<Food> foods = businessLogicLayer.FindFoodsByProducts(token, start, size, productIds);
                response = new BaseResponse<List<Food>>(foods);
            }
            catch (Exception ex)
            {
                response = new BaseResponse<List<Food>> { Error = ex.Message };
            }
            return response;
        }

        private List<Food> TypeBasedSearchFoods(string token, FoodType foodType, int userId, string searchName, int start, int size)
        {
            List<Food> foods = null;
            switch (foodType)
            {
                case FoodType.ADDED:
                        foods = businessLogicLayer.SearchAddedFoods(token, userId, searchName, start, size);
                        break;
                case FoodType.LIKE:
                        foods = businessLogicLayer.SearchLikeFoods(token, userId, searchName, start, size);
                        break;
                case FoodType.RECOMMENDED:
                        foods = businessLogicLayer.SearchRecommendedFoods(token, userId, searchName, start, size);
                        break;
                case FoodType.COMPOSED:
                    throw new MissingMethodException("Please, use 'food/search/products' api");
            }
            return foods;
        }
    }
}
