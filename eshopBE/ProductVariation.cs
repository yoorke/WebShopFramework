using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace eshopBE
{
    public class ProductVariation
    {
        private int _variationID;
        private string _value;
        private string _color;
        
        public int VariationID
        {
            get { return _variationID; }
            set { _variationID = value; }
        }

        public string Value
        {
            get { return _value; }
            set { _value = value; }
        }

        public string Color
        {
            get { return _color; }
            set { _color = value; }
        }
    }
}
