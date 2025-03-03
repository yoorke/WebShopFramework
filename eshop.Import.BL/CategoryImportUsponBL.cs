using eshop.Import.BE;
using eshop.Import.BL.AbstractClasses;
using eshop.Import.DL;
using eshop.Import.DL.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace eshop.Import.BL
{
    public class CategoryImportUsponBL : BaseCategoryImportBL
    {
        private ICategoryImportDL _categoryImportDL = new CategoryImportUsponDL();
        private IProductImportDL _productImportDL = new ProductImportUsponDL();

        public CategoryImportUsponBL() : base(new CategoryImportUsponDL(), "uspon")
        {

        }

        public override void UpdateSupplierCategories()
        {
            GetParameter getParameter = new GetParameter()
            {
                Attributes = false,
                Description = false,
                Images = false
            };

            XmlDocument xmlDoc = _productImportDL.DownloadProducts("", "", getParameter);

            if (xmlDoc != null && xmlDoc.DocumentElement != null)
            {
                XmlNodeList productList = xmlDoc.DocumentElement.SelectNodes("artikal");

                foreach (XmlNode xmlProduct in productList)
                {
                    if (xmlProduct.SelectSingleNode("nadgrupa") != null)
                    {
                        _categoryImportDL.SaveSupplierCategory(xmlProduct.SelectSingleNode("nadgrupa").InnerText.Trim(), null);

                        if (xmlProduct.SelectSingleNode("grupa") != null)
                        {
                            _categoryImportDL.SaveSupplierCategory(xmlProduct.SelectSingleNode("grupa").InnerText.Trim(), xmlProduct.SelectSingleNode("nadgrupa").InnerText.Trim());
                        }
                    }
                }
            }
        }
    }
}
