using Microsoft.EntityFrameworkCore;
using StepToFoodServer.Models;

namespace StepToFoodServer.Database.Configurations
{
    public class AuthorFoodConfiguration : IFoodConfiguration
    {
        public void Configure(ModelBuilder builder)
        {
            builder.Entity<Food>()
                .HasOne(food => food.Author)
                .WithMany(author => author.AddedFoods)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}