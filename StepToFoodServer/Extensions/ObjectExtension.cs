using System;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;

namespace StepToFoodServer.Utils
{
    public static class ObjectExtension
    {
        public static T Clone<T>(this T obj)
        {
            var inst = obj.GetType().GetMethod("MemberwiseClone", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);

            return (T)inst?.Invoke(obj, null);
        }
    }
}
