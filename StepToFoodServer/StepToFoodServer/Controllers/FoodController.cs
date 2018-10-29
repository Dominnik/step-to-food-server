using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using StepToFoodServer.Database;
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
                int foodId = int.Parse(Request.Query["foodId"]);
                Food food = businessLogicLayer.FoodWithProducts(foodId);
                response = new BaseResponse<Food>(food);
            }
            catch (Exception ex)
            {
                response = new BaseResponse<Food>();
                response.Error = ex.Message;
            }
            return response;
        }

        [HttpGet("get/image")]
        public BaseResponse<FileContentResult> GetImage()
        {
            BaseResponse<FileContentResult> response = null;
            try
            {
                int foodId = int.Parse(Request.Query["foodId"]);
                byte[] image = Convert.FromBase64String(foodRepository.Get(foodId).Image);
                response = new BaseResponse<FileContentResult>(File(image, "image/jpeg"));
            }
            catch (Exception ex)
            {
                response = new BaseResponse<FileContentResult> { Error = ex.Message };
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

                businessLogicLayer.AddFoodWithProducts(user, food);
                response = new BaseResponse<int>(0);
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
                businessLogicLayer.Check(token);

                int foodId = int.Parse(Request.Query["foodId"]);
                foodRepository.Delete(foodId);
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
                businessLogicLayer.LikeForFood(user, foodId, hasLike);
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

        [HttpGet("search")]
        public BaseResponse<List<Food>> Search()
        {
            BaseResponse<List<Food>> response = null;
            try
            {
                int userId = int.Parse(Request.Query["userId"]);
                int startId = int.Parse(Request.Query["startId"]);
                int size = int.Parse(Request.Query["size"]);
                string searchName = Request.Query["searchName"];
                FoodType foodType = (FoodType)Enum.Parse(typeof(FoodType), Request.Query["foodType"]);

                List<Food> foods = TypeBasedSearchFoods(foodType, userId, searchName, startId, size);
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
                int startId = int.Parse(Request.Query["startId"]);
                int size = int.Parse(Request.Query["size"]);

                List<Food> foods = businessLogicLayer.FindFoodsByProducts(startId, size, productIds);
                response = new BaseResponse<List<Food>>(foods);
            }
            catch (Exception ex)
            {
                response = new BaseResponse<List<Food>> { Error = ex.Message };
            }
            return response;
        }

        private List<Food> TypeBasedSearchFoods(FoodType foodType, int userId, string searchName, int startId, int size)
        {
            List<Food> foods = null;
            switch (foodType)
            {
                case FoodType.ADDED:
                        foods = businessLogicLayer.SearchAddedFoods(userId, searchName, startId, size);
                        break;
                case FoodType.LIKE:
                        foods = businessLogicLayer.SearchLikeFoods(userId, searchName, startId, size);
                        break;
                case FoodType.RECOMMENDED:
                        foods = businessLogicLayer.SearchRecommendedFoods(userId, searchName, startId, size);
                        break;
                case FoodType.COMPOSED:
                    throw new MissingMethodException("Please, use 'food/search/products' api");
            }
            return foods;
        }
    }
}
