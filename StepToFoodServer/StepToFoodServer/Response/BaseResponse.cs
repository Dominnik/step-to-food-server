using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StepToFoodServer.Response
{
    public class BaseResponse<T>
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public bool Success { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public T Content { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Error { get; set; }

        public BaseResponse()
        {
            Success = false;
            Error = "Empty Response";
        }

        public BaseResponse(T content, string error = null, bool success = true)
        {
            Content = content;
            Success = success;
            Error = error;
        }
    }
}
