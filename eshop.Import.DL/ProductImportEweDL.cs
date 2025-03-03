using eshop.Import.BE;
using eshop.Import.DL.AbstractClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Xml;

namespace eshop.Import.DL
{
    public class ProductImportEweDL : BaseProductImportDL
    {
        public ProductImportEweDL() : base("ewe")
        {

        }

        protected override XmlDocument getXml(string category, string subcategory, GetParameter getParameter)
        {
            XmlDocument xmlDoc = new XmlDocument();
            StringBuilder url = new StringBuilder();
            SupplierSettings supplierSettings = this.getSupplierSettings("ewe");

            url.Append($"{supplierSettings.Url}?user={supplierSettings.Username}&secretcode={supplierSettings.Password}");

            if(getParameter.Images)
            {
                url.Append("&images=1");
            }

            if(getParameter.Attributes)
            {
                url.Append("&attributes=1");
            }

            if(!string.IsNullOrEmpty(getParameter.Category))
            {
                url.Append($"&category={HttpUtility.UrlEncode(category)}");
            }

            if(getParameter.SubCategories != null
                && getParameter.SubCategories.Count > 0
                && !string.IsNullOrEmpty(getParameter.SubCategories[0]))
            {
                url.Append($"&subcategory={getParameter.SubCategories[0]}");
            }

            xmlDoc = getDataFromUrl(url.ToString());

            return xmlDoc;
        }
    }
}
