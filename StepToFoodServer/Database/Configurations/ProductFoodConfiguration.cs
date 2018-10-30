using Microsoft.EntityFrameworkCore;
using StepToFoodServer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StepToFoodServer.Database.Configurations
{
    public class ProductFoodConfiguration : IFoodConfiguration
    {
        public void Configure(ModelBuilder builder)
        {
            builder.Entity<ProductFood>()
                .HasKey(productFood => new { productFood.ProductId, productFood.FoodId });

            builder.Entity<ProductFood>()
                .HasOne(productFood => productFood.Product)
                .WithMany(product => product.ProductFoods)
                .HasForeignKey(productFood => productFood.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<ProductFood>()
                .HasOne(productFood => productFood.Food)
                .WithMany(food => food.ProductFoods)
                .HasForeignKey(productFood => productFood.FoodId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
