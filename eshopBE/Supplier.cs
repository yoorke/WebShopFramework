using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace eshopBE
{
    public class Supplier
    {
        private int _supplierID;
        private string _name;

        public Supplier()
        {
        }

        public Supplier(int supplierID, string name)
        {
            _supplierID = supplierID;
            _name = name;
        }

        public int SupplierID
        {
            get { return _supplierID; }
            set { _supplierID = value; }
        }

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }
    }
}
