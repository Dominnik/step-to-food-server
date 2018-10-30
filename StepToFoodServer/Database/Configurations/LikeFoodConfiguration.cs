using Microsoft.EntityFrameworkCore;
using StepToFoodServer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StepToFoodServer.Database.Configurations
{
    public class LikeFoodConfiguration : IFoodConfiguration
    {
        public void Configure(ModelBuilder builder)
        {
            builder.Entity<LikeFood>()
                .HasKey(likeFood => new { likeFood.UserId, likeFood.FoodId });

            builder.Entity<LikeFood>()
                .HasOne(likeFood => likeFood.User)
                .WithMany(user => user.LikeFoods)
                .HasForeignKey(likeFood => likeFood.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<LikeFood>()
                .HasOne(likeFood => likeFood.Food)
                .WithMany(food => food.LikeFoods)
                .HasForeignKey(likeFood => likeFood.FoodId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
