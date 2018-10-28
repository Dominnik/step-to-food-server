using StepToFoodServer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StepToFoodServer.Repositories
{
    public interface IRepository<T> where T : Entity
    {
        List<T> GetAll();

        T Get(long id);

        void Add(T user);

        void Update(T user);

        void Delete(long id);
    }
}
