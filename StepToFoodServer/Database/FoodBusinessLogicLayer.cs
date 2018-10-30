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
                throw new ArgumentException("wrong login or password");

            user.AddedFoods = null;
            user.Avatar = null;
            return user;
        }

        public void ChangePassword(string token, string password, string newPassword)
        {
            User user = userRepository
                .Filter(elem => elem.Token == token)
                .First();

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
                .First();

            if (user == null)
                throw new ArgumentException("wrong login or password");

            user.Token = null;
            userRepository.Update(user);
        }

        public List<Product> FindProductsByFood(int foodId)
        {
            List<Product> products = productRepository
                .Filter(elem => elem.ProductFoods.Any(food => food.FoodId == foodId))
                .ToList();

            return products;
        }

        public List<Product> FindProductsByName(string name)
        {
            List<Product> products = productRepository
                .Filter(elem => elem.Name.Contains(name))
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
                user.LikeFoods.Add(likeFood);
                //food.LikeFoods.Add(likeFood);
                userRepository.Add(user);
            }
            else
            {
                LikeFood likeFood = user.LikeFoods
                    .Where(elem => elem.FoodId == foodId)
                    .Single();
                user.LikeFoods.Remove(likeFood);
            }
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
            return foods
                .OrderBy(food => -food.Products.Where(product => (bool)product.IncludedInSearch).Count())
                .ThenBy(food => food.Products.Where(product => !(bool)product.IncludedInSearch).Count())
                .Skip(start)
                .Take(size)
                .ToList();
        }

        public List<Food> SearchAddedFoods(int userId, string searchName, int start, int size)
        {
            List<Food> foods = foodRepository
                .Filter(food => food != null && food.Author.Id == userId && food.Name.Contains(searchName))
                .Skip(start)
                .Take(size)
                .ToList();

            foreach (var food in foods)
                SetProducts(food);
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
                SetProducts(food);
            return foods;
        }

        public List<Food> SearchRecommendedFoods(int userId, string searchName, int start, int size)
        {
            List<Food> foods = RecommendedFoodsExists(userId) ?
                GetRecommendedFoods(userId, searchName) : 
                foodRepository.Filter(food => food.Name.Contains(searchName));

            foods = foods.Skip(start).Take(size).ToList();
            foreach (var food in foods)
                SetProducts(food);

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

            food.Products = productRepository
                .Filter(product => product.ProductFoods.Any(elem => elem.FoodId == food.Id))
                .ToList();
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
                    new ProductFood(productFromDb, food) : productFood);
            }
            food.ProductFoods = productFoods;
        }
    }
}
