﻿using StepToFoodServer.Models;
using StepToFoodServer.Repositories;
using StepToFoodServer.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StepToFoodServer.Database
{
    public class FoodBusinessLogicLayer : IBusinessLogicLayer
    {
        private readonly FoodContext context;
        private readonly IRepository<Food> foodRepository;
        private readonly IRepository<User> userRepository;
        private readonly IRepository<Product> productRepository;

        public FoodBusinessLogicLayer(
            FoodContext context,
            IRepository<Food> foodRepository, 
            IRepository<User> userRepository, 
            IRepository<Product> productRepository)
        {
            this.context = context;
            this.foodRepository = foodRepository;
            this.userRepository = userRepository;
            this.productRepository = productRepository;
        }

        public int Register(string name, string login, string password)
        {
            if (LoginExists(login))
                throw new ArgumentException("User with this login exists");
            
            User user = new User(name, login, password);
            userRepository.Add(user);
            return user.Id;
        }

        public User Login(string login, string password)
        {
            User user = userRepository
                .Filter(elem => elem.Login == login && elem.Password == password)
                .First();

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
                .First();

            if (user == null)
                throw new UnauthorizedAccessException("Invalid security token");

            user.AddedFoods = null;
            user.Avatar = null;
            return user;
        }

        public void ChangePassword(string token, string password, string newPassword)
        {
            User user = Check(token);
            if (user.Password != password)
                throw new ArgumentException("wrong current password");

            user.Password = newPassword;
            userRepository.Update(user);
        }

        public void Terminate(string token)
        {
            User user = userRepository
                .Filter(elem => elem.Token == token)
                .First();

            if (user == null)
                throw new ArgumentException("wrong login or password");

            user.Token = null;
            userRepository.Update(user);
        }

        public List<Product> FindProductsByFood(int foodId)
        {
            List<Product> products = productRepository
                .Filter(elem => elem.ProductFoods.Any(productFood => productFood.FoodId == foodId))
                .ToList();

            foreach(var product in products)
            {
                product.Weight = product.ProductFoods
                    .Where(productFood => productFood.FoodId == foodId)
                    .Single().Weight;
            }
            return products;
        }

        public List<Product> FindProductsByName(string name)
        {
            return productRepository
                .Filter(elem => elem.Name.ToLower().Contains(name.ToLower()))
                .ToList();
        }

        public Food FoodWithProducts(int foodId)
        {
            Food food = foodRepository.Get(foodId);
            food.Image = ImageLink.GetFoodImageLink(food.Id);
            SetProducts(food);
            return food;
        }

        public int AddFoodWithProducts(User user, Food food)
        {
            food.Author = user;
            foodRepository.Add(food);
            foreach (Product product in food.Products)
            {
                Product productFromDb = productRepository.Get(product.Id);
                ProductFood productFood = new ProductFood(productFromDb, food, (int)product.Weight);
                context.ProductFoods.Add(productFood);
            }
            context.SaveChanges();
            return food.Id;
        }

        public void UpdateFoodWithProducts(Food food)
        {
            Food foodFromDb = foodRepository.Get(food.Id);
            foodFromDb.Image = food.Image;
            foodFromDb.Name = food.Name;
            foodFromDb.Description = food.Description;
            foodFromDb.Calorie = food.Calorie;
            foodFromDb.Protein = food.Protein;
            foodFromDb.Fat = food.Fat;
            foodFromDb.Carbohydrates = food.Carbohydrates;
            foodFromDb.Products = food.Products;
            SetProductFoods(foodFromDb);
        }

        public void LikeForFood(User user, int foodId, bool hasLike)
        {
            Food food = foodRepository.Get(foodId);
            if (hasLike)
            {
                LikeFood likeFood = new LikeFood(user, food);
                context.LikeFoods.Add(likeFood);
            }
            else
            {
                LikeFood likeFood = user.LikeFoods
                    .Where(elem => elem.FoodId == foodId)
                    .Single();
                context.LikeFoods.Remove(likeFood);
            }
            context.SaveChanges();
        }

        public List<Food> FindFoodsByProducts(int start, int size, List<int> productIds)
        {
            List<Food> foods = foodRepository.Filter(food => FoodContainsAnyProduct(food, productIds));
            foreach (var food in foods)
            {
                SetProducts(food);
                foreach (var product in food.Products)
                    product.IncludedInSearch = productIds.Contains(product.Id);
            }
            foods = foods
               .OrderBy(food => -food.Products.Where(product => (bool)product.IncludedInSearch).Count())
               .ThenBy(food => food.Products.Where(product => !(bool)product.IncludedInSearch).Count())
               .Skip(start)
               .Take(size)
               .ToList();

            foreach (var food in foods)
                food.Image = ImageLink.GetFoodImageLink(food.Id);
            return foods;
        }

        public List<Food> SearchAddedFoods(int userId, string searchName, int start, int size)
        {
            List<Food> foods = foodRepository
                .Filter(food => food != null && food.Author.Id == userId && food.Name.Contains(searchName))
                .Skip(start).ToList()
                .Take(size)
                .ToList();

            foreach (var food in foods)
            {
                food.Image = ImageLink.GetFoodImageLink(food.Id);
                SetProducts(food);
            }
            return foods;
        }
        
        public List<Food> SearchLikeFoods(int userId, string searchName, int start, int size)
        {
            List<Food> foods = foodRepository
                .Filter(food => UserPutLikeFood(userId, food) && food.Name.Contains(searchName))
                .Skip(start)
                .Take(size)
                .ToList();

            foreach (var food in foods)
            {
                food.Image = ImageLink.GetFoodImageLink(food.Id);
                SetProducts(food);
            }
            return foods;
        }

        public List<Food> SearchRecommendedFoods(int userId, string searchName, int start, int size)
        {
            List<Food> foods = RecommendedFoodsExists(userId) ?
                GetRecommendedFoods(userId, searchName) : 
                foodRepository.Filter(food => food.Name.Contains(searchName));

            foods = foods.Skip(start).Take(size).ToList();
            foreach (var food in foods)
            {
                food.Image = ImageLink.GetFoodImageLink(food.Id);
                SetProducts(food);
            }

            return foods;
        }

        private bool LoginExists(string login)
        {
            return userRepository.Filter(user => user.Login == login).Count != 0;
        }

        private bool WrongLoginOrPassword(string login, string password)
        {
            return userRepository.Filter(user => user.Login == login && user.Password == password).Count != 0;
        }

        private bool FoodContainsAnyProduct(Food food, List<int> productIds)
        {
            return food.ProductFoods.Any(productFood => productIds.Contains(productFood.ProductId));
        }

        private bool UserPutLikeFood(int userId, Food food)
        {
            return food.LikeFoods.Any(likeFood => likeFood.UserId == userId);
        }

        private List<Food> GetRecommendedFoods(int userId, string searchName)
        {
            List<Food> likeFoods = LikeFoods(userId);
            List<User> recommendedUsers = userRepository.Filter(user => user.Id != userId &&
                user.LikeFoods.Any(likeFood => likeFood.Food.Name.Contains(searchName) &&
                UserPutLikeFood(userId, likeFood.Food)));
            recommendedUsers = recommendedUsers
                .OrderBy(user => -LikeFoods(user.Id).Intersect(likeFoods).Count())
                .ToList();

            List<Food> recommendedFoods = new List<Food>();
            foreach (var recommendedUser in recommendedUsers)
            {
                List<Food> likeFoodsFromRecommendedUser = LikeFoods(recommendedUser.Id);
                likeFoodsFromRecommendedUser.RemoveAll(food => likeFoods.Contains(food));
                recommendedFoods.AddRange(likeFoodsFromRecommendedUser);
            }
            return recommendedFoods;
        }

        private bool RecommendedFoodsExists(int userId)
        {
            return GetRecommendedFoods(userId, "").Count != 0;
        }

        private List<Food> LikeFoods(int userId)
        {
            return foodRepository.Filter(food => food.LikeFoods.Any(likeFood => likeFood.User.Id == userId));
        }

        private void SetProducts(Food food)
        {
            if (food == null)
                return;

            food.Products = FindProductsByFood(food.Id);
        }

        private void SetProductFoods(Food food)
        {
            List<ProductFood> productFoods = new List<ProductFood>();
            foreach (Product product in food.Products)
            {
                Product productFromDb = productRepository.Get(product.Id);
                var productFood = food.ProductFoods
                    .Where(elem => elem.ProductId == product.Id)
                    .Single();

                productFoods.Add(productFood == null ? 
                    new ProductFood(productFromDb, food, (int)product.Weight) : productFood);
            }
            food.ProductFoods = productFoods;
        }
    }
}
