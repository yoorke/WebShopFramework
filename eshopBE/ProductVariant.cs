using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eshopBE
{
    public class ProductVariant
    {
        public int ID { get; set; }
        public string Code { get; set; }
        public double Price { get; set; }
        public int ProductID { get; set; }
        public bool IsInStock { get; set; }
        public double Quantity { get; set; }
        public List<AttributeValue> Attributes { get; set; }
    }
}
