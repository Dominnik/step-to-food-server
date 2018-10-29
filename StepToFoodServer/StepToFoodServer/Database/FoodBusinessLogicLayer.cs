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

        public User Check(string token)
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

        public List<Product> FindProductsByFood(int foodId)
        {
            List<Product> products = productRepository
                .Filter(elem => elem.ProductFoods.Any(food => food.FoodId == foodId))
                .DefaultIfEmpty()
                .ToList();

            return products;
        }

        public List<Product> FindProductsByName(string name)
        {
            List<Product> products = productRepository
                .Filter(elem => elem.Name.Contains(name))
                .DefaultIfEmpty()
                .ToList();

            return products;
        }

        public Food FoodWithProducts(int foodId)
        {
            Food food = foodRepository.Get(foodId);
            SetProducts(food);
            return food;
        }

        public void AddFoodWithProducts(User user, Food food)
        {
            foreach (Product product in food.Products)
            {
                ProductFood productFood = new ProductFood(product, food);
                food.ProductFoods.Add(productFood);
                //product.ProductFoods.Add(productFood);
            }
            food.Author = user;
            //user.AddedFoods.Add(food);
            foodRepository.Add(food);
        }

        public void LikeForFood(User user, int foodId, bool hasLike)
        {
            Food food = foodRepository.Get(foodId);

            LikeFood likeFood = new LikeFood(user, food);
            user.LikeFoods.Add(likeFood);
            //food.LikeFoods.Add(likeFood);
            userRepository.Add(user);
        }

        private bool LoginExists(string login)
        {
            return userRepository.Filter(user => user.Login == login).Count != 0;
        }

        private bool WrongLoginOrPassword(string login, string password)
        {
            return userRepository.Filter(user => user.Login == login && user.Password == password).Count != 0;
        }

        private void SetProducts(Food food)
        {
            food.Products = productRepository
                .Filter(product => product.ProductFoods.Any(elem => elem.FoodId == food.Id))
                .DefaultIfEmpty()
                .ToList();
        }
    }
}
