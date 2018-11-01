using StepToFoodServer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace StepToFoodServer.Repositories
{
    public class ProductRepository : IRepository<Product>
    {
        private readonly FoodContext context;

        public ProductRepository(FoodContext context)
        {
            this.context = context;
        }

        public List<Product> GetAll()
        {
            return context.Products.ToList();
        }

        public Product Get(long id)
        {
            return context.Products.First(t => t.Id == id);
        }

        public void Add(Product product)
        {
            context.Products.Add(product);
            context.SaveChanges();
        }

        public void Update(Product product)
        {
            context.Products.Update(product);
            context.SaveChanges();
        }

        public void Delete(long id)
        {
            var entity = Get(id);
            context.Products.Remove(entity);
            context.SaveChanges();
        }

        public List<Product> Filter(Func<Product, bool> predicate)
        {
            return GetAll().Where(predicate).ToList();
        }
    }
}
