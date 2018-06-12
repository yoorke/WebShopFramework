using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace eshopBE
{
    public class Brand
    {
        private int _brandID;
        private string _name;
        private string _logoUrl;

        public Brand()
        {
        }

        public Brand(int brandID, string name, string logoUrl)
        {
            _brandID = brandID;
            _name = name;
            _logoUrl = logoUrl;
        }

        public int BrandID
        {
            get { return _brandID; }
            set { _brandID = value; }
        }

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public string LogoUrl
        {
            get { return _logoUrl; }
            set { _logoUrl = value; }
        }
    }
}
