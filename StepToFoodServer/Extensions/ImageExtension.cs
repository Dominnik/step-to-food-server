using System;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;

namespace StepToFoodServer.Utils
{
    public static class ImageExtension
    {
        public static string ToBase64(this Image image)
        {
            using (MemoryStream imageStream = new MemoryStream())
            {
                image.Save(imageStream, ImageFormat.Jpeg);
                byte[] imageBytes = imageStream.ToArray();
                return Convert.ToBase64String(imageBytes);
            }
        }

        public static Image FromBase64(this Image image, string base64String)
        {
            byte[] imageBytes = Convert.FromBase64String(base64String);
            using (MemoryStream imageStream = new MemoryStream(imageBytes, 0, imageBytes.Length))
            {
                imageStream.Write(imageBytes, 0, imageBytes.Length);
                return Image.FromStream(imageStream, true);
            }
        }

        public static byte[] ToArray(this Image image)
        {
            using (MemoryStream imageStream = new MemoryStream())
            {
                image.Save(imageStream, ImageFormat.Jpeg);
                return imageStream.ToArray();
            }
        }

        public static Image FromArray(this Image image, byte[] imageBytes)
        {
            using (MemoryStream imageStream = new MemoryStream(imageBytes, 0, imageBytes.Length))
            {
                imageStream.Write(imageBytes, 0, imageBytes.Length);
                return Image.FromStream(imageStream, true);
            }
        }
    }
}
