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
                retails.Insert(0, new Retail(-1, null, string.Empty, string.Empty, string.Empty, string.Empty, "Sve", false));

            return retails;
        }

        public List<string> GetDistinct(bool addSelect)
        {
            List<string> retails = new RetailDL().GetDistinct();

            if (addSelect)
                retails.Insert(0, "Svi");

            return retails;
        }

        public Retail GetRetail(int retailID)
        {
            return new RetailDL().GetRetail(retailID);
        }

        public int Insert(Retail retail)
        {
            return new RetailDL().Insert(retail);
        }

        public int Update(Retail retail)
        {
            return new RetailDL().Update(retail);
        }

        public int Delete(int retailID)
        {
            return new RetailDL().Delete(retailID);
        }
    }
}
