using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eshopBE
{
    public class ProductSimple
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }

        public ProductSimple(int id, string name, string code)
        {
            ID = id;
            Name = name;
            Code = code;
        }
    }
}
