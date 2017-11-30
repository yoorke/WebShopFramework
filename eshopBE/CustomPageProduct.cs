using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace eshopBE
{
    [Serializable]
    public class CustomPageProduct
    {
        public int ProductID { get; set; }
        public string ProductName { get; set; }

        public CustomPageProduct(int productID, string name)
        {
            ProductID = productID;
            ProductName = name;
        }
    }
}
