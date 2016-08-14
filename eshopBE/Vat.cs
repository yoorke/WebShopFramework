using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace eshopBE
{
    public class Vat
    {
        private int _vatID;
        private int _vat;

        public Vat()
        {
        }

        public Vat(int vatID, int vat)
        {
            _vatID = vatID;
            _vat = vat;
        }

        public int VatID
        {
            get { return _vatID; }
            set { _vatID = value; }
        }

        public int VatValue
        {
            get { return _vat; }
            set { _vat = value; }
        }
    }
}
