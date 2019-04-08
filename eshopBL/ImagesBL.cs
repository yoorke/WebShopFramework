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
                }
                catch(Exception ex)
                {
                    ErrorLog.LogError(ex);
                }
            }
        }
    }
}
