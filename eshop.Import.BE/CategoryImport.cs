using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eshop.Import.BE
{
    public class CategoryImport
    {
        public int ID { get; set; }
        public int SupplierID { get; set; }
        public string Name { get; set; }
        public int ParentID { get; set; }
        public string ParentName { get; set; }
        public bool IsSelected;
        public DateTime InsertDate { get; set; }
    }
}
