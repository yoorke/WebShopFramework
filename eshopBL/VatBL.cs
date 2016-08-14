using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eshopBE;
using eshopDL;

namespace eshopBL
{
    public class VatBL
    {
        public List<Vat> GetVats()
        {
            VatDL vatDL = new VatDL();
            return vatDL.GetVats();
        }
    }
}
