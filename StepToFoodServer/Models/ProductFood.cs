namespace StepToFoodServer.Models
{
    public class ProductFood
    {
        public int ProductId { get; set; }
        public Product Product { get; set; }
        public int FoodId { get; set; }
        public Food Food { get; set; }

        public ProductFood() { }

        public ProductFood(Product product, Food food)
        {
            Product = product;
            Food = food;

            ProductId = product.Id;
            FoodId = food.Id;
        }
    }
}
