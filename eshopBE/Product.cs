using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace eshopBE
{
    public class Product
    {
        private int _productID;
        private string _code;
        private string _supplierCode;
        private Brand _brand;
        private string _name;
        private string _description;
        private double _price;
        private double _webPrice;
        private List<ProductImage> _images;
        private List<AttributeValue> _attributes;
        private List<Category> _categories;
        private int _supplierID;
        private bool _isApproved;
        private bool _isActive;
        private int _vatID;
        private DateTime _insertDate;
        private DateTime _updateDate;
        private string _specification;
        private bool _isLocked;
        private bool _isInStock;
        private string _ean;
        private Promotion _promotion;
        private double _supplierPrice;
        private UnitOfMeasure _unitOfMeasure;
        private string _fullCategoryUrl;
        private bool _isPriceLocked;
        private string _declaration;
        private double _weight;
        private int? _weightRangeID;
        private bool _allowUserImageUpload;
        private string _comment;
        private bool _canBeDelivered;
        private bool _hasVariants = true;
        private string _shortDescription;
        private bool _isFreeDelivery;
        private string _manualUrl;
        private string _listDescription;
        private int _sortIndex;

        public int ProductID
        {
            get { return _productID; }
            set { _productID = value; }
        }

        public string Code
        {
            get { return _code; }
            set { _code = value; }
        }

        public string SupplierCode
        {
            get { return _supplierCode; }
            set { _supplierCode = value; }
        }

        public Brand Brand
        {
            get { return _brand; }
            set { _brand = value; }
        }

        public string Name
        {
            get
            {
                if(bool.Parse(ConfigurationManager.AppSettings["productNameToLowerCase"]))
                {
                    return toLowerCase(_name);
                }

                return _name;
            }
            set { _name = value; }
        }

        public string Description
        {
            get { return _description; }
            set { _description = value; }
        }

        public double Price
        {
            get { return _price; }
            set { _price = value; }
        }

        public double WebPrice
        {
            get { return _webPrice; }
            set { _webPrice = value; }
        }

        public List<ProductImage> Images
        {
            get { return _images; }
            set { _images = value; }
        }

        public List<AttributeValue> Attributes
        {
            get { return _attributes; }
            set { _attributes = value; }
        }

        public List<Category> Categories
        {
            get { return _categories; }
            set { _categories = value; }
        }

        public int SupplierID
        {
            get { return _supplierID; }
            set { _supplierID = value; }
        }

        public bool IsApproved
        {
            get { return _isApproved; }
            set { _isApproved = value; }
        }

        public bool IsActive
        {
            get { return _isActive; }
            set { _isActive = value; }
        }

        public int VatID
        {
            get { return _vatID; }
            set { _vatID = value; }
        }

        public DateTime InsertDate
        {
            get { return _insertDate; }
            set { _insertDate = value; }
        }

        public DateTime UpdateDate
        {
            get { return _updateDate; }
            set { _updateDate = value; }
        }

        public string Specification
        {
            get { return _specification; }
            set { _specification = value; }
        }

        public bool IsLocked
        {
            get { return _isLocked; }
            set { _isLocked = value; }
        }

        public bool IsInStock
        {
            get { return _isInStock; }
            set { _isInStock = value; }
        }

        public string Ean
        {
            get { return _ean; }
            set { _ean = value; }
        }

        public Promotion Promotion
        {
            get { return _promotion; }
            set { _promotion = value; }
        }

        public string FullCategoryUrl
        {
            get { return _fullCategoryUrl; }
            set { _fullCategoryUrl = value; }
        }

        public string Url
        {
            get
            {
                if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["productUrlDefinition"]))
                {
                    string[] properties = ConfigurationManager.AppSettings["productUrlDefinition"].Split(',');
                    StringBuilder url = new StringBuilder();
                    url.Append(bool.Parse(ConfigurationManager.AppSettings["includeParentUrlInCategoryUrl"]) ? this._fullCategoryUrl  + "/" : _categories[0].Name + "/");
                    foreach (string property in properties)
                    {
                        if (property.Contains("."))
                        {
                            object value = this.GetType().GetProperty(property.Substring(0, property.IndexOf('.'))).GetValue(this, null);
                            url.Append(value.GetType().GetProperty(property.Substring(property.IndexOf('.') + 1)).GetValue(value, null).ToString() + "-");
                        }
                        else
                            url.Append(this.GetType().GetProperty(property).GetValue(this, null) + "-");
                    }
                    url.Append("-" + _productID.ToString());
                    return "/proizvodi/" + CreateFriendlyUrl(url.ToString());
                }
                else
                {
                    string url = bool.Parse(ConfigurationManager.AppSettings["fullProductUrl"]) ? (bool.Parse(ConfigurationManager.AppSettings["includeParentUrlInCategoryUrl"]) ? this._fullCategoryUrl + "/" : _categories[0].Name + "/") + _brand.Name + " " : string.Empty;
                    url += _name.Replace('/', '-') + "-" + _productID.ToString();
                    //return "/proizvodi/" + CreateFriendlyUrl(_categories[0].Name + "/" + _brand.Name + " " + _name.Replace('/','-') + "-" + _productID);
                    url = (url.Contains("proizvodi") ? string.Empty : "/proizvodi/") + CreateFriendlyUrl(url);
                    if (!url.StartsWith("/"))
                        url = "/" + url;
                    return url;
                }
            }
        }

        public string FullName
        {
            get
            {
                if(bool.Parse(ConfigurationManager.AppSettings["productNameToLowerCase"]))
                {
                    return _brand.Name + " " + toLowerCase(_name);
                }

                return _brand.Name + " " + _name;
            }
        }

        public UnitOfMeasure UnitOfMeasure
        {
            get { return _unitOfMeasure; }
            set { _unitOfMeasure = value; }
        }

        public static string CreateFriendlyUrl(string url)
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

        public double SupplierPrice
        {
            get { return _supplierPrice; }
            set { _supplierPrice = value; }
        }

        public bool IsPriceLocked
        {
            get { return _isPriceLocked; }
            set { _isPriceLocked = value; }
        }

        public string Declaration
        {
            get { return _declaration; }
            set { _declaration = value; }
        }

        public double Weight
        {
            get { return _weight; }
            set { _weight = value; }
        }

        public int? WeightRangeID
        {
            get { return _weightRangeID; }
            set { _weightRangeID = value; }
        }

        public bool AllowUserImageUpload
        {
            get { return _allowUserImageUpload; }
            set { _allowUserImageUpload = value; }
        }

        public string Comment
        {
            get { return _comment; }
            set { _comment = value; }
        }

        public bool CanBeDelivered
        {
            get { return _canBeDelivered; }
            set { _canBeDelivered = value; }
        }

        public bool HasVariants
        {
            get { return _hasVariants; }
            set { _hasVariants = value; }
        }

        public string ShortDescription
        {
            get { return _shortDescription; }
            set { _shortDescription = value; }
        }

        public bool IsFreeDelivery
        {
            get { return _isFreeDelivery; }
            set { _isFreeDelivery = value; }
        }

        public string ManualUrl
        {
            get { return _manualUrl; }
            set { _manualUrl = value; }
        }

        public string ListDescription
        {
            get { return _listDescription; }
            set { _listDescription = value; }
        }

        public int SortIndex
        {
            get { return _sortIndex; }
            set { _sortIndex = value; }
        }

        private string toLowerCase(string content)
        {
            string loweredContent = content.ToLower();
            loweredContent = loweredContent.Substring(0, 1).ToUpper() + loweredContent.Substring(1);

            return loweredContent;
        }
    }

    public class ProductUpdatePrice
    {
        public int ID { get; set; }
        public bool IsLocked { get; set; }
        public bool IsPriceLocked { get; set; }
        public int BrandID { get; set; }
        public int CategoryID { get; set; }
    }
}
