using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace eshopBE
{
    public class PromotionPrice
    {
        private int _promotionID;
        private double _price;
        private string _imageUrl;

        public int PromotionID
        {
            get { return _promotionID; }
            set { _promotionID = value; }
        }

        public double Price
        {
            get { return _price; }
            set { _price = value; }
        }

        public string ImageUrl
        {
            get { return _imageUrl; }
            set { _imageUrl = value; }
        }
    }
}
