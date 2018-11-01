using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StepToFoodServer.Models.Binds
{
    public class FormDataCollection
    {
        public string Name { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
        public string NewPassword { get; set; }
        public string Token { get; set; }
    }
}
