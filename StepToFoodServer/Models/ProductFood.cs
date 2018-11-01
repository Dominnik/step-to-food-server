namespace StepToFoodServer.Models
{
    public class ProductFood
    {
        public int ProductId { get; set; }
        public virtual Product Product { get; set; }
        public int FoodId { get; set; }
        public virtual Food Food { get; set; }
        public int Weight { get; set; }

        public ProductFood() { }

        public ProductFood(Product product, Food food, int weight)
        {
            Product = product;
            Food = food;

            ProductId = product.Id;
            FoodId = food.Id;

            Weight = weight;
        }
    }
}
