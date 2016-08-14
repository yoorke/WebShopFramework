using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace eshopBE
{
    public class Promotion
    {
        private int _promotionID;
        private string _name;
        private double _value;
        private string _imageUrl;
        private double _price;
        private bool _showOnFirstPage;
        private DateTime _dateFrom;
        private DateTime _dateTo;
        private string _url;
        private bool _shonOnMenu;

        public Promotion()
        {
        }

        public Promotion(int promotionID, string name, double value, string imageUrl, double price, bool showOnFirstPage, DateTime dateFrom, DateTime dateTo, string url, bool showOnMenu)
        {
            _name = name;
            _value = value;
            _promotionID = promotionID;
            _imageUrl = imageUrl;
            _price = price;
            _showOnFirstPage = showOnFirstPage;
            _dateFrom = dateFrom;
            _dateTo = dateTo;
            _url = url;
            _shonOnMenu = showOnMenu;
        }

        public int PromotionID
        {
            get { return _promotionID; }
            set { _promotionID = value; }
        }

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public double Value
        {
            get { return _value; }
            set { _value = value; }
        }

        public string ImageUrl
        {
            get { return _imageUrl; }
            set { _imageUrl = value; }
        }

        public double Price
        {
            get { return _price; }
            set { _price = value; }
        }

        public bool ShowOnFirstPage
        {
            get { return _showOnFirstPage; }
            set { _showOnFirstPage = value; }
        }

        public DateTime DateFrom
        {
            get { return _dateFrom; }
            set { _dateFrom = value; }
        }

        public DateTime DateTo
        {
            get { return _dateTo; }
            set { _dateTo = value; }
        }

        public string Url
        {
            get { return _url; }
            set { _url = value; }
        }

        public bool ShowOnMenu
        {
            get { return _shonOnMenu; }
            set { _shonOnMenu = value; }
        }
    }
}
