using eshop.Import.BE;
using eshop.Import.DL.AbstractClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Xml;

namespace eshop.Import.DL
{
    public class ProductImportDSCDL : BaseProductImportDL
    {
        public ProductImportDSCDL() : base("dsc")
        {

        }

        protected override XmlDocument getXml(string category, string subCategory, GetParameter getParameter)
        {
            XmlDocument xmlDoc = new XmlDocument();
            StringBuilder url = new StringBuilder();
            SupplierSettings supplierSettings = getSupplierSettings("dsc");

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls | SecurityProtocolType.Ssl3;

            url.Append($"{supplierSettings.Url}?korisnickoime={supplierSettings.Username}&lozinka={supplierSettings.Password}");

            if(!string.IsNullOrEmpty(category))
            {
                url.Append($"&kategorija={HttpUtility.UrlEncode(category)}");
            }

            if (getParameter.Images)
            {
                url.Append("&slike=1");
            }

            if(getParameter.Attributes)
            {
                url.Append("&karakteristike=1");
            }

            if(getParameter.Description)
            {
                url.Append("&opis=1");
            }

            //xmlDoc.Load(url.ToString());
            xmlDoc = getDataFromUrl(url.ToString());

            return xmlDoc;
        }
    }
}
