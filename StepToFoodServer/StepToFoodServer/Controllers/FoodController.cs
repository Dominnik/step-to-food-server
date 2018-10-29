using System;
using System.Collections.Generic;
using System.Linq;
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
    }
}
