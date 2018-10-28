using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StepToFoodServer.Database;
using StepToFoodServer.Models;
using StepToFoodServer.Repositories;

namespace StepToFoodServer.Controllers
{
    [Route("api/[controller]")]
    public class ProductController : Controller
    {
        private readonly IBusinessLogicLayer businessLogicLayer;
        private readonly IRepository<Product> productRepository;

        public ProductController(IBusinessLogicLayer businessLogicLayer, IRepository<Product> productRepository)
        {
            this.businessLogicLayer = businessLogicLayer;
            this.productRepository = productRepository;
        }

        // GET api/product
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "product1", "product2" };
        }

        // GET api/product/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "product";
        }

        // POST api/product
        [HttpPost]
        public void Post([FromBody]string value)
        {
        }

        // PUT api/product/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/product/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}