﻿using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Proxies;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
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
            services.AddMvc().AddJsonOptions(
                options => options.SerializerSettings.ReferenceLoopHandling = 
                Newtonsoft.Json.ReferenceLoopHandling.Ignore);

            services.AddSingleton<IFoodConfiguration, AuthorFoodConfiguration>();
            services.AddSingleton<IFoodConfiguration, ProductFoodConfiguration>();
            services.AddSingleton<IFoodConfiguration, LikeFoodConfiguration>();

            var connection = Configuration["Sqlite:Connection"];
            services.AddDbContext<FoodContext>(options => options.UseLazyLoadingProxies().UseSqlite(connection));

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
