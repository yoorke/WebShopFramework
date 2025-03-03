using eshopBE;
using eshopBLInterfaces;
using eshopUtilities;
using eshopUtilities.Extensions;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace eshopBL
{
    public class ImageConvertor : IImageConvertor
    {
        ImageProcessor imageProc;

        public ImageConvertor()
        {
            imageProc = new ImageProcessor();
        }

        public string ConvertImageToWebP(string imagePath)
        {
            try
            {
                FileInfo fileInfo = new FileInfo(HttpContext.Current.Server.MapPath("~/" + imagePath));

                if(!fileInfo.Exists)
                {
                    return string.Empty;
                }

                Image image = null;
                string filePath = fileInfo.FullName;
                string extension = fileInfo.Extension.Substring(1).ToLower();
                string name = fileInfo.Name.Substring(0, fileInfo.Name.LastIndexOf('.'));
                string newFilePath = filePath.Substring(0, fileInfo.FullName.LastIndexOf('.') + 1) + VVSImageType.WebP.ToString().ToLower();
            
                if(extension.Equals(VVSImageType.WebP.ToString().ToLower()))
                {
                    return string.Empty;
                }

                image = Image.FromFile(filePath);
                image.SaveWebp(newFilePath);

                imageProc.SaveThumb(image,
                                    int.Parse(ConfigurationManager.AppSettings["mainWidth"]),
                                    int.Parse(ConfigurationManager.AppSettings["mainHeight"]),
                                    ConfigurationManager.AppSettings["mainName"],
                                    filePath.Substring(0, filePath.LastIndexOf('.')),
                                    fileInfo.Extension, VVSImageType.WebP);

                imageProc.SaveThumb(image,
                                    int.Parse(ConfigurationManager.AppSettings["listWidth"]),
                                    int.Parse(ConfigurationManager.AppSettings["listHeight"]),
                                    ConfigurationManager.AppSettings["listName"],
                                    filePath.Substring(0, filePath.LastIndexOf('.')),
                                    fileInfo.Extension, VVSImageType.WebP);

                imageProc.SaveThumb(image,
                                    int.Parse(ConfigurationManager.AppSettings["thumbWidth"]),
                                    int.Parse(ConfigurationManager.AppSettings["thumbHeight"]),
                                    ConfigurationManager.AppSettings["thumbName"],
                                    filePath.Substring(0, filePath.LastIndexOf('.')),
                                    fileInfo.Extension, VVSImageType.WebP);

                image.Dispose();

                return $"{name}.{VVSImageType.WebP.ToString().ToLower()}";
            }
            catch(Exception ex)
            {
                ErrorLog.LogError(ex);
                return string.Empty;
            }
        }
    }
}
