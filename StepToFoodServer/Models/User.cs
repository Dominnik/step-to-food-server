using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StepToFoodServer.Models
{
    public class User : Entity
    {
        public string Name { get; set; }

        [JsonIgnore]
        public string Login { get; set; }

        [JsonIgnore]
        public string Password { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Avatar { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Token { get; set; }

        [JsonIgnore]
        public virtual ICollection<Food> AddedFoods { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public virtual ICollection<LikeFood> LikeFoods { get; set; }

        public User() { }

        public User(string name, string login, string password)
        {
            Name = name;
            Login = login;
            Password = password;
        }
    }
}
