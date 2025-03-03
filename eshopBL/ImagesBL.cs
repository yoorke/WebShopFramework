using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Web;
using eshopBE;
using System.Data;
using eshopUtilities;
using eshopDL;
using System.Drawing;
using System.Configuration;
using eshopUtilities.Extensions;

namespace eshopBL
{
    public class ImagesBL
    {
        public string[] GetAllImageFiles()
        {
            return Directory.GetFiles(HttpContext.Current.Server.MapPath("~/images/p"), "*.*", SearchOption.AllDirectories);
        }

        public DataTable GetUnusedImageFiles()
        {
            string[] allFiles = GetAllImageFiles();
            List<ProductImage> unusedFiles = new List<ProductImage>();
            string tempFilename = string.Empty;
            bool add = false;

            DataTable imageUrls = new DataTable();
            imageUrls.Columns.Add("imageUrl");
            imageUrls.Columns.Add("filename");
            imageUrls.Columns.Add("file");
            imageUrls.Columns.Add("shortFilename");
            imageUrls.Columns.Add("size");

            foreach(string file in allFiles)
            {
                string filename = file.Substring(file.LastIndexOf("\\") + 1);
                string shortFilename = filename;
                if (filename.IndexOf("-") > -1)
                    filename = filename.Substring(0, filename.IndexOf("-")) + filename.Substring(filename.IndexOf("."));
                FileInfo fi = new FileInfo(file);
                
                //if(filename != tempFilename)
                //{ 
                //if (filename != tempFilename)
                //{
                //add = !new ProductBL().ImageExistsInDatabase(filename);
                try { 
                    DataRow row = imageUrls.NewRow();
                    row["imageUrl"] = new ProductBL().CreateImageDirectory(int.Parse(filename.Substring(0, filename.IndexOf('.')))) + file.Substring(file.LastIndexOf("\\") + 1);
                    row["filename"] = filename;
                    row["file"] = file;
                    row["shortFilename"] = shortFilename;
                    row["size"] = fi.Length.ToString();
                    imageUrls.Rows.Add(row);
                }
                catch(Exception ex)
                { }
                //}
                //if(add)
                //{ 
                //try
                //{ 
                //string url = new ProductBL().CreateImageDirectory(int.Parse(filename.Substring(0, filename.IndexOf('.')))) + filename;
                //unusedFiles.Add(new ProductImage(url, 0, file));
                //}
                //catch(Exception ex)
                //{ }
                //}

                //}
                tempFilename = filename;
            }

            return new ProductBL().ImagesTableExistsInDatabase(imageUrls);

            //return unusedFiles;
        }

        public void DeleteUnusedFiles()
        {
            DataTable unusedFiles = GetUnusedImageFiles();
            foreach(DataRow file in unusedFiles.Rows)
            {
                try
                {
                    //File.Delete(HttpContext.Current.Server.MapPath("~/images/p/" + file["file"]));
                    File.Delete(file["file"].ToString());
                    ErrorLog.LogMessage("Deleted: " + file["file"].ToString());

                    deleteFolder(file["file"].ToString());
                }
                catch(Exception ex)
                {
                    ErrorLog.LogError(ex);
                }
            }
        }

        public void deleteFolder(string file)
        {
            try
            {
                string folder = file.Substring(0, file.LastIndexOf('\\'));
                if (Directory.Exists(folder))
                {
                    if(Directory.GetFiles(folder, "*.*").Count() == 0)
                    {
                        Directory.Delete(folder);
                    }
                }
            }
            catch(Exception ex)
            {
                ErrorLog.LogError(ex);
            }
        }

        public DataTable GetAllProductImages()
        {
            return new ProductDL().GetAllProductImages();
        }

        public void ResizeProductImages(int width, int height)
        {
            DataTable images = GetAllProductImages();
            ProductBL productBL = new ProductBL();
            string fullName = string.Empty;

            foreach(DataRow image in images.Rows)
            {
                try
                { 
                    string name = image["imageUrl"].ToString().Substring(0, image["imageUrl"].ToString().LastIndexOf('.'));
                    string extension = image["imageUrl"].ToString().Substring(image["imageUrl"].ToString().LastIndexOf('.') + 1);
                    fullName = productBL.CreateImageDirectory(int.Parse(name)) + image["imageUrl"];

                    Image original = Image.FromFile(fullName);
                    Image resized = Common.CreateThumb(original, width, height);
                    resized.Save(fullName.Substring(0, fullName.LastIndexOf('.')) + "-" + width.ToString() + "x" + height.ToString() + extension);
                }
                catch(Exception ex)
                {
                    ErrorLog.LogError(new Exception("Error while resizing image: " + fullName, ex));
                }
            }
        }

        public bool DeleteImageFile(string imageUrl)
        {
            try
            {
                FileInfo fileInfo = new FileInfo(HttpContext.Current.Server.MapPath("~/" + imageUrl));

                if(!fileInfo.Exists)
                {
                    return false;
                }

                string name = fileInfo.FullName.Substring(0, fileInfo.FullName.LastIndexOf('.'));
                string extension = fileInfo.Extension.Substring(1);

                try
                {
                    fileInfo.Delete();
                    //File.Delete(fileInfo.FullName);
                }
                catch (Exception ex)
                {
                    ErrorLog.LogError(ex);
                }

                try
                {
                    File.Delete($"{name}-{ConfigurationManager.AppSettings["mainName"]}.{extension}");
                }
                catch(Exception ex)
                {
                    ErrorLog.LogError(ex);
                }

                try
                {
                    File.Delete($"{name}-{ConfigurationManager.AppSettings["listName"]}.{extension}");
                }
                catch(Exception ex)
                {
                    ErrorLog.LogError(ex);
                }

                try
                {
                    File.Delete($"{name}-{ConfigurationManager.AppSettings["thumbName"]}.{extension}");
                }
                catch(Exception ex)
                {
                    ErrorLog.LogError(ex);
                }
            }
            catch(Exception ex)
            {
                ErrorLog.LogError(ex);
                throw;
            }

            return true;
        }

        public void ResizeProductImages(int productID)
        {
            ProductDL productDL = new ProductDL();
            ProductBL productBL = new ProductBL();
            string filename;
            int imageID;
            string fullPath;

            List<ProductImage> images = productDL.GetProductImages(productID);

            foreach(var image in images)
            {
                imageID = int.Parse(image.ImageUrl.Substring(0, image.ImageUrl.LastIndexOf('.')));
                filename = productBL.CreateImageDirectory(imageID) + image.ImageUrl;
                fullPath = HttpContext.Current.Server.MapPath($"~{filename}");

                productBL.CreateProductImages(fullPath);
            }
        }

        public bool IsImageRezolutionSifficient(string path)
        {
            bool isSufficient = false;
            //string filename = path.Substring(path.LastIndexOf('\\') + 1);
            //filename = filename.Substring(0, filename.LastIndexOf('.'));
            string extension = path.Substring(path.LastIndexOf('.') + 1);
            Image image = null;
            int preferredWidth = int.Parse(ConfigurationManager.AppSettings["preferredImageWidth"]);
            int preferredHeight = int.Parse(ConfigurationManager.AppSettings["preferredImageHeight"]);

            if(!bool.Parse(ConfigurationManager.AppSettings["checkImageSize"]))
            {
                return true;
            }

            switch(extension)
            {
                case "jpg":
                case "jpeg":
                case "png":
                    {
                        image = Image.FromFile(path);
                        break;
                    }
                case "webp":
                    {
                        image = WebpImage.FromFile(path);
                        break;
                    }
            }

            if(image != null && image.Width >= preferredWidth && image.Height >= preferredHeight)
            {
                isSufficient = true;
            }

            image.Dispose();
            image = null;

            return isSufficient;
        }
    }
}
