namespace StepToFoodServer.Models
{
    public class ProductFood
    {
        public int ProductId { get; set; }
        public Product Product { get; set; }
        public int FoodId { get; set; }
        public Food Food { get; set; }
    }
}
