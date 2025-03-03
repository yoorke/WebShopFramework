using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eshopBE
{
    public class ProductVariantAttributeValue
    {
        public int AttributeID { get; set; }
        public int AttributeValueID { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
        public string ValueCode { get; set; }
        public int ProductVariantID { get; set; }
    }
}
