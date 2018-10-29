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

        User Check(string token);

        void ChangePassword(string token, string password, string newPassword);

        void Terminate(string token);

        List<Product> FindProductsByFood(int foodId);

        List<Product> FindProductsByName(string name);

        Food FoodWithProducts(int foodId);

        void AddFoodWithProducts(User user, Food food);

        void LikeForFood(User user, int foodId, bool hasLike);

        void UpdateFoodWithProducts(Food food);

        List<Food> FindFoodsByProducts(int startId, int size, List<int> productIds);

        List<Food> SearchAddedFoods(int userId, string searchName, int startId, int size);

        List<Food> SearchLikeFoods(int userId, string searchName, int startId, int size);

        List<Food> SearchRecommendedFoods(int userId, string searchName, int startId, int size);
    }
}
