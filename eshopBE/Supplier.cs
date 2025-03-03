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
        private string _code;
        private string _currencyCode;

        public Supplier()
        {
        }

        public Supplier(int supplierID, string name)
        {
            _supplierID = supplierID;
            _name = name;
        }

        public Supplier(int supplierID, string name, string currencyCode)
        {
            _supplierID = supplierID;
            _name = name;
            _currencyCode = currencyCode;
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

        public string Code
        {
            get { return _code; }
            set { _code = value; }
        }

        public string CurrencyCode
        {
            get { return _currencyCode; }
            set { _currencyCode = value; }
        }
    }
}
