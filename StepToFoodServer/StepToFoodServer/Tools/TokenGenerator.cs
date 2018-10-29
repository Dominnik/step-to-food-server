using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StepToFoodServer
{
    public static class TokenGenerator
    {
        public static string Generate()
        {
            return Guid.NewGuid().ToString();
        }
    }
}
