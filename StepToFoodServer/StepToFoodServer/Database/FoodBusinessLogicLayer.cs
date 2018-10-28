using StepToFoodServer.Models;
using StepToFoodServer.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StepToFoodServer.Database
{
    public class FoodBusinessLogicLayer : IBusinessLogicLayer
    {
        private readonly IRepository<Food> foodRepository;
        private readonly IRepository<User> userRepository;
        private readonly IRepository<Product> productRepository;

        public FoodBusinessLogicLayer(IRepository<Food> foodRepository, IRepository<User> userRepository, IRepository<Product> productRepository)
        {
            this.foodRepository = foodRepository;
            this.userRepository = userRepository;
            this.productRepository = productRepository;
        }

        public void Register(string name, string login, string password)
        {
            if (LoginExists(login))
                throw new ArgumentException("User with this login exists");
            
            User user = new User(name, login, password);
            userRepository.Add(user);
        }

        public User Login(string login, string password)
        {
            User user = userRepository
                .Filter(elem => elem.Login == login && elem.Password == password)
                .FirstOrDefault();

            if (user == null)
                throw new ArgumentException("wrong login or password");
            user.Token = TokenGenerator.Generate();
            userRepository.Update(user);

            user.AddedFoods = null;
            user.Avatar = null;
            return user;
        }

        public User Login(string token)
        {
            User user = userRepository
                .Filter(elem => elem.Token == token)
                .FirstOrDefault();

            if (user == null)
                throw new ArgumentException("wrong login or password");

            user.AddedFoods = null;
            user.Avatar = null;
            return user;
        }

        public void ChangePassword(string token, string password, string newPassword)
        {
            User user = userRepository
                .Filter(elem => elem.Token == token)
                .FirstOrDefault();

            if (user == null)
                throw new ArgumentException("wrong login or password");

            if (user.Password != password)
                throw new ArgumentException("wrong current password");

            user.Password = newPassword;
            userRepository.Update(user);
        }

        public void Terminate(string token)
        {
            User user = userRepository
                .Filter(elem => elem.Token == token)
                .FirstOrDefault();

            if (user == null)
                throw new ArgumentException("wrong login or password");

            user.Token = null;
            userRepository.Update(user);
        }

        private bool LoginExists(string login)
        {
            return userRepository.Filter(user => user.Login == login).Count != 0;
        }

        private bool WrongLoginOrPassword(string login, string password)
        {
            return userRepository.Filter(user => user.Login == login && user.Password == password).Count != 0;
        }
    }
}
