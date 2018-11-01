using Microsoft.EntityFrameworkCore;
using StepToFoodServer.Database.Configurations;
using StepToFoodServer.Models;
using System.Collections.Generic;

namespace StepToFoodServer
{
    public class FoodContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Food> Foods { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductFood> ProductFoods { get; set; }
        public DbSet<LikeFood> LikeFoods { get; set; }

        private readonly IEnumerable<IFoodConfiguration> configurations;

        public FoodContext(DbContextOptions<FoodContext> options, IEnumerable<IFoodConfiguration> configurations) : base(options)
        {
            this.configurations = configurations;
            Database.Migrate();
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            foreach (var configuration in configurations)
                configuration.Configure(builder);
            base.OnModelCreating(builder);
        }
    }
}