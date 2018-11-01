using StepToFoodServer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace StepToFoodServer.Repositories
{
    public interface IRepository<T> where T : Entity
    {
        List<T> GetAll();

        T Get(long id);

        void Add(T entity);

        void Update(T entity);

        void Delete(long id);

        List<T> Filter(Func<T, bool> predicate);
    }
}
