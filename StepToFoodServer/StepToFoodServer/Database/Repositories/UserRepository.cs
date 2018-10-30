using StepToFoodServer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace StepToFoodServer.Repositories
{
    public class UserRepository : IRepository<User>
    {
        private readonly FoodContext context;

        public UserRepository(FoodContext context)
        {
            this.context = context;
        }

        public List<User> GetAll()
        {
            return context.Users.ToList();
        }

        public User Get(long id)
        {
            return context.Users.First(t => t.Id == id);
        }

        public void Add(User user)
        {
            context.Users.Add(user);
            context.SaveChanges();
        }

        public void Update(User user)
        {
            context.Users.Update(user);
            context.SaveChanges();
        }

        public void Delete(long id)
        {
            var entity = Get(id);
            context.Users.Remove(entity);
            context.SaveChanges();
        }

        public List<User> Filter(Expression<Func<User, bool>> predicate)
        {
            return context.Users.Where(predicate).ToList();
        }
    }
}
