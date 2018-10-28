using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StepToFoodServer.Database.Configurations
{
    public interface IFoodConfiguration
    {
        void Configure(ModelBuilder builder);
    }
}
