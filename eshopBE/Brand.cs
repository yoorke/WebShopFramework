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

        public Brand()
        {
        }

        public Brand(int brandID, string name)
        {
            _brandID = brandID;
            _name = name;
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
    }
}
