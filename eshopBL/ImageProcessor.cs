using eshopBE;
using eshopUtilities;
using eshopUtilities.Extensions;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eshopBL
{
    public class ImageProcessor
    {
        public void SaveThumb(Image original, int width, int height, string type, string filename, string extension)
        {
            Image thumb = Common.CreateThumb(original, width, height);

            if (!bool.Parse(ConfigurationManager.AppSettings["useWebpImages"]))
            {
                thumb.Save($"{filename}-{type}{extension}");
            }
            else
            {
                thumb.SaveWebp($"{filename}-{type}.webp");
            }
        }

        public void SaveThumb(Image original, int width, int height, string type, string filename, string extension, VVSImageType imageType)
        {
            Image thumb = Common.CreateThumb(original, width, height);

            switch(imageType)
            {
                case VVSImageType.Jpg:
                case VVSImageType.Png:
                { 
                    thumb.Save($"{filename}-{type}{extension}");
                    break;
                }
                case VVSImageType.WebP:
                {
                    thumb.SaveWebp($"{filename}-{type}.webp");
                    break;
                }
            }
        }
    }
}
