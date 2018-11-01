using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StepToFoodServer.Utils
{
    public static class ImageLink
    {
        public static string GetFoodImageLink(int foodId)
        {
            return "food/get/image?foodId=" + foodId;
        }

        public static string GetUserAvatarLink(int userId)
        {
            return "user/get/avatar?userId=" + userId;
        }
    }
}
