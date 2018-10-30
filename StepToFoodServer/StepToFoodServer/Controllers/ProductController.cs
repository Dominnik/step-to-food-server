using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StepToFoodServer.Database;
using StepToFoodServer.Models;
using StepToFoodServer.Repositories;
using StepToFoodServer.Response;

namespace StepToFoodServer.Controllers
{
    [Route("[controller]")]
    public class ProductController : Controller
    {
        private readonly IBusinessLogicLayer businessLogicLayer;
        private readonly IRepository<Product> productRepository;

        public ProductController(IBusinessLogicLayer businessLogicLayer, IRepository<Product> productRepository)
        {
            this.businessLogicLayer = businessLogicLayer;
            this.productRepository = productRepository;
        }

        [HttpGet]
        public BaseResponse<Product> Get()
        {
            BaseResponse<Product> response = null;
            try
            {
                int productId = int.Parse(Request.Query["productId"]);
                Product product = productRepository.Get(productId);
                response = new BaseResponse<Product>(product);
            }
            catch (Exception ex)
            {
                response = new BaseResponse<Product> { Error = ex.Message };
            }
            return response;
        }

        [HttpGet("all")]
        public BaseResponse<List<Product>> GetAll()
        {
            BaseResponse<List<Product>> response = null;
            try
            {
                List<Product> products = productRepository.GetAll();
                response = new BaseResponse<List<Product>>(products);
            }
            catch (Exception ex)
            {
                response = new BaseResponse<List<Product>> { Error = ex.Message };
            }
            return response;
        }

        [HttpGet("search/food")]
        public BaseResponse<List<Product>> GetByFood()
        {
            BaseResponse<List<Product>> response = null;
            try
            {
                int foodId = int.Parse(Request.Query["search"]);
                List<Product> products = businessLogicLayer.FindProductsByFood(foodId);
                response = new BaseResponse<List<Product>>(products);
            }
            catch (Exception ex)
            {
                response = new BaseResponse<List<Product>> { Error = ex.Message };
            }
            return response;
        }

        [HttpGet("search/name")]
        public BaseResponse<List<Product>> GetByName()
        {
            BaseResponse<List<Product>> response = null;
            try
            {
                string name = (string)Request.Query["search"] ?? "";
                List<Product> products = businessLogicLayer.FindProductsByName(name);
                response = new BaseResponse<List<Product>>(products);
            }
            catch (Exception ex)
            {
                response = new BaseResponse<List<Product>> { Error = ex.Message };
            }
            return response;
        }
    }
}