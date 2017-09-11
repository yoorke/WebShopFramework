using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eshopDL;
using eshopBE;

namespace eshopBL
{
    public class RetailBL
    {
        public List<Retail> GetRetails(int cityID, string retailName, bool addSelect)
        {
            List<Retail> retails = new RetailDL().GetRetails(cityID, retailName);

            if (addSelect)
                retails.Insert(0, new Retail(-1, null, string.Empty, string.Empty, string.Empty, string.Empty, "Sve"));

            return retails;
        }

        public List<string> GetDistinct(bool addSelect)
        {
            List<string> retails = new RetailDL().GetDistinct();

            if (addSelect)
                retails.Insert(0, "Svi");

            return retails;
        }
    }
}
