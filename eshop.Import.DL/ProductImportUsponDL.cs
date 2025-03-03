using eshop.Import.BE;
using eshop.Import.DL.AbstractClasses;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Xml;

namespace eshop.Import.DL
{
    public class ProductImportUsponDL : BaseProductImportDL
    {
        public ProductImportUsponDL() : base("uspon")
        {

        }

        protected override XmlDocument getXml(string category, string subCategory, GetParameter getParameter)
        {
            XmlDocument xmlDoc = new XmlDocument();
            StringBuilder url = new StringBuilder();
            SupplierSettings supplierSettings = this.getSupplierSettings("uspon");

            url.Append($"{supplierSettings.Url}?korisnickoime={supplierSettings.Username}&lozinka={supplierSettings.Password}");

            if(!string.IsNullOrEmpty(category))
            {
                url.Append($"&kategorija={HttpUtility.UrlEncode(category)}");
            }

            if (getParameter.Images)
            {
                url.Append("&slike=1");
            }

            if (getParameter.Description)
            {
                url.Append("&opis=1");
            }

            if(getParameter.Attributes)
            {
                url.Append("&karakteristike=1");
            }

            if(getParameter.InStock)
            {
                url.Append("&kolicina=1");
            }

            if(getParameter.Short)
            {
                url.Append("&kratak=1");
            }

            //xmlDoc.Load(url.ToString());
            xmlDoc = getDataFromUrl(url.ToString());

            return xmlDoc;
        }
    }
}
