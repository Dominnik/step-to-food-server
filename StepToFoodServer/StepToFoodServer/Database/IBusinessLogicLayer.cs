using StepToFoodServer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StepToFoodServer.Database
{
    public interface IBusinessLogicLayer
    {
        void Register(string name, string login, string password);

        User Login(string login, string password);

        User Login(string token);

        void ChangePassword(string token, string password, string newPassword);

        void Terminate(string token);
    }
}
