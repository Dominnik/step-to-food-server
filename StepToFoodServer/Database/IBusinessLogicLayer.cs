using StepToFoodServer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StepToFoodServer.Database
{
    public interface IBusinessLogicLayer
    {
        int Register(string name, string login, string password);

        User Login(string login, string password);

        User Check(string token);

        void ChangePassword(string token, string password, string newPassword);

        void Terminate(string token);

        List<Product> FindProductsByFood(int foodId);

        List<Product> FindProductsByName(string name);

        Food FoodWithProducts(string token, int foodId);

        int AddFoodWithProducts(int userId, Food food);

        void LikeForFood(int userId, int foodId, bool hasLike);

        void UpdateFoodWithProducts(Food food);

        List<Food> FindFoodsByProducts(string token, int start, int size, List<int> productIds);

        List<Food> SearchAddedFoods(string token, int userId, string searchName, int start, int size);

        List<Food> SearchLikeFoods(string token, int userId, string searchName, int start, int size);

        List<Food> SearchRecommendedFoods(string token, int userId, string searchName, int start, int size);
    }
}
