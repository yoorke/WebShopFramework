using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Imazen.WebP;

namespace eshopUtilities.Extensions
{
    public static class ImageExtensions
    {
        public static Image FromWebp(this Image image, string path)
        {
            Bitmap bitmap = null;

            return (Image)bitmap;
        }

        public static void SaveWebp(this Image image, string path)
        {
            using (var saveStream = File.Open(path, FileMode.Create))
            {
                new SimpleEncoder().Encode((Bitmap)image, saveStream, 50);
            }
        }
    }
}
