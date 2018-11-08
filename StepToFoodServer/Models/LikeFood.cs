namespace StepToFoodServer.Models
{
    public class LikeFood
    {
        public int UserId { get; set; }
        public virtual User User { get; set; }
        public int FoodId { get; set; }
        public virtual Food Food { get; set; }

        public LikeFood() { }

        public LikeFood(int userId, int foodId)
        {
            UserId = userId;
            FoodId = foodId;
        }
    }
}
