namespace StepToFoodServer.Models
{
    public class LikeFood
    {
        public int UserId { get; set; }
        public User User { get; set; }
        public int FoodId { get; set; }
        public Food Food { get; set; }

        public LikeFood() { }

        public LikeFood(User user, Food food)
        {
            User = user;
            Food = food;

            UserId = user.Id;
            FoodId = food.Id;
        }
    }
}
