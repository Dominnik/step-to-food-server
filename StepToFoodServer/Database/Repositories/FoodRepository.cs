using StepToFoodServer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace StepToFoodServer.Repositories
{
    public class FoodRepository : IRepository<Food>
    {
        private readonly FoodContext context;

        public FoodRepository(FoodContext context)
        {
            this.context = context;
        }

        public List<Food> GetAll()
        {
            return context.Foods.ToList();
        }

        public Food Get(long id)
        {
            return context.Foods.First(t => t.Id == id);
        }
        
        public void Add(Food food)
        {
            context.Foods.Add(food);
            context.SaveChanges();
        }

        public void Update(Food food)
        {
            context.Foods.Update(food);
            context.SaveChanges();
        }

        public void Delete(long id)
        {
            var entity = Get(id);
            context.Foods.Remove(entity);
            context.SaveChanges();
        }

        public List<Food> Filter(Expression<Func<Food, bool>> predicate)
        {
            return context.Foods.Where(predicate).ToList();
        }
    }
}
