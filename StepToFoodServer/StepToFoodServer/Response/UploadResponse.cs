using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StepToFoodServer.Response
{
    public class UploadResponse
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public bool Success { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Link { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Message { get; set; }

        public UploadResponse(string message = "Empty Response", bool success = false)
        {
            Success = success;
            Message = message;
        }

        public UploadResponse(string link, string message = null, bool success = true)
        {
            Link = link;
            Success = success;
            Message = message;
        }
    }
}
