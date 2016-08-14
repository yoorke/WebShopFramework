using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
        private List<string> _images;
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
            get { return _name; }
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

        public List<string> Images
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

        public string Url
        {
            get { return "/proizvodi/" + CreateFriendlyUrl(_categories[0].Name + "/" + _brand.Name + " " + _name.Replace('/','-') + "-" + _productID); }
        }

        public string FullName
        {
            get { return _brand.Name + " " + _name; }
        }

        public static string CreateFriendlyUrl(string url)
        {
            url = url.ToLower();
            char[] notAllwed = { 'š', 'ć', 'č', 'ž', ',', '.', '"', ' ', '(', ')', '&', '+', '%', '$' };
            char[] replacement = { 's', 'c', 'c', 'z', '-', '-', '-', '-', '-', '-', '-', '-', '-', '-' };

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
    }
}
