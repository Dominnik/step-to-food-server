using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using StepToFoodServer.Database;
using StepToFoodServer.Database.Configurations;
using StepToFoodServer.Models;
using StepToFoodServer.Repositories;

namespace StepToFoodServer
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();

            services.AddSingleton<IFoodConfiguration, AuthorFoodConfiguration>();
            services.AddSingleton<IFoodConfiguration, ProductFoodConfiguration>();
            services.AddSingleton<IFoodConfiguration, LikeFoodConfiguration>();

            services.AddDbContext<FoodContext>(options => options.UseSqlite(@"Data Source=StepToFood.db"));
            
            services.AddScoped(typeof(IRepository<User>), typeof(UserRepository));
            services.AddScoped(typeof(IRepository<Food>), typeof(FoodRepository));
            services.AddScoped(typeof(IRepository<Product>), typeof(ProductRepository));

            services.AddScoped<IBusinessLogicLayer, FoodBusinessLogicLayer>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMvc();
        }
    }
}
