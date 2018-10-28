using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace StepToFoodServer.Models
{
    public class Food : Entity
    {
        public string Name { get; set; }
        public string Image { get; set; }
        public string Description { get; set; }
        public double Calorie { get; set; }
        public double Protein { get; set; }
        public double Fat { get; set; }
        public double Carbohydrates { get; set; }
        public User Author { get; set; }

        [JsonIgnore]
        public ICollection<ProductFood> ProductFoods { get; set; }

        [JsonIgnore]
        public ICollection<LikeFood> LikeFoods { get; set; }

        [NotMapped]
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public ICollection<Product> Products { get; set; }

        [NotMapped]
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public bool? IsYourAdded { get; set; }

        [NotMapped]
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public bool? HasYourLike { get; set; }
    }
}
