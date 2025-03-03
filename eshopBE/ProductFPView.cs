using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eshopBE
{
    public class ProductFPView
    {
        private string _name;
        public int ProductID { get; set; }
        public string Code { get; set; }
        public string BrandName { get; set; }
        public string Name
        {
            get
            {
                if (bool.Parse(ConfigurationManager.AppSettings["productNameToLowerCase"]))
                {
                    return toLowerCase(_name);
                }

                return _name;
            }
            set
            {
                _name = value;
            }
        }
        public double Price { get; set; }
        public double WebPrice { get; set; }
        public double PromotionPrice { get; set; }
        public string ImageUrl { get; set; }
        public bool IsInStock { get; set; }
        public string FullCategoryUrl { get; set; }
        public string CategoryName { get; set; }
        public string PromotionImageUrl { get; set; }
        public string Url
        {
            get
            {
                if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["productUrlDefinition"]))
                {
                    string[] properties = ConfigurationManager.AppSettings["productUrlDefinition"].Split(',');
                    StringBuilder sbUrl = new StringBuilder();
                    sbUrl.Append(bool.Parse(ConfigurationManager.AppSettings["includeParentUrlInCategoryUrl"]) ? this.FullCategoryUrl + "/" : CategoryName + "/");

                    foreach (string property in properties)
                    {
                        if (property.Contains("."))
                        {
                            object value = this.GetType().GetProperty(property.Substring(0, property.IndexOf('.'))).GetValue(this, null);
                            sbUrl.Append(value.GetType().GetProperty(property.Substring(property.IndexOf('.') + 1)).GetValue(value, null).ToString() + "-");
                        }
                        else
                        { 
                            sbUrl.Append(this.GetType().GetProperty(property).GetValue(this, null) + "-");
                        }
                    }
                    sbUrl.Append("-" + ProductID.ToString());
                    return "/proizvodi/" + createFriendlyUrl(sbUrl.ToString());
                }
                else
                {
                    string url = bool.Parse(ConfigurationManager.AppSettings["fullProductUrl"]) ?
                    (bool.Parse(ConfigurationManager.AppSettings["includeParentUrlInCategoryUrl"]) ?
                        this.FullCategoryUrl + "/" : this.CategoryName + "/") + this.BrandName + " "
                        : string.Empty;
                    url += this._name.Replace('/', '-') + "-" + this.ProductID.ToString();
                    return "/proizvodi/" + createFriendlyUrl(url);
                }
            }
            set { }
        }
        public bool CanBeDelivered { get; set; }
        public bool HasVariants { get; set; } = true;
        public bool IsFreeDelivery { get; set; }
        public string ListDescription { get; set; }
        public int SortIndex { get; set; }

        private string createFriendlyUrl(string url)
        {
            url = url.ToLower();
            char[] notAllwed = { 'š', 'ć', 'č', 'ž', ',', '.', '"', ' ', '(', ')', '&', '+', '%', '$', '*', '<', '>', ':', '±', ':' };
            char[] replacement = { 's', 'c', 'c', 'z', '-', '-', '-', '-', '-', '-', '-', '-', '-', '-', '-', '-', '-', '-', '-', '-' };

            url = url.Replace("\n", "-");
            url = url.Replace("\r", "-");
            for (int i = 0; i < notAllwed.Length; i++)
                url = url.Replace(notAllwed[i], replacement[i]);

            url = url.Replace("đ", "dj");

            url = url.Replace("--", "-");
            url = url.Replace("---", "-");
            url = url.Trim();

            return url;
        }

        private string toLowerCase(string content)
        {
            string loweredContent = content.ToLower();

            loweredContent = loweredContent.Substring(0, 1).ToUpper() + loweredContent.Substring(1);

            return loweredContent;
        }
    }
}
