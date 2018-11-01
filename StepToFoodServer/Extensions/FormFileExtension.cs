using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace StepToFoodServer.Extensions
{
    public static class FormFileExtension
    {
        public static string ToBase64String(this IFormFile file)
        {
            if (file.Length <= 0)
                return null;

            using (var stream = new MemoryStream())
            {
                file.CopyTo(stream);
                var fileBytes = stream.ToArray();
                return Convert.ToBase64String(fileBytes);
            }
        }
    }
}
