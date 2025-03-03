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
    public class CategoryImportEweV2BL : BaseCategoryImportBL
    {
        private ICategoryImportDL _categoryImportDL = new CategoryImportEweV2DL();
        private IProductImportDL _productImportDL = new ProductImportEweV2DL();

        public CategoryImportEweV2BL() : base(new CategoryImportEweV2DL(), "ewe")
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

            if(xmlDoc != null && xmlDoc.DocumentElement != null)
            {
                XmlNodeList productList = xmlDoc.DocumentElement.SelectNodes("product");

                foreach(XmlNode xmlProduct in productList)
                {
                    if(xmlProduct.SelectSingleNode("acCategory") != null)
                    {
                        _categoryImportDL.SaveSupplierCategory(xmlProduct.SelectSingleNode("acCategory").InnerText.Trim(), null);

                        if(xmlProduct.SelectSingleNode("acSubCategory") != null)
                        {
                            _categoryImportDL.SaveSupplierCategory(xmlProduct.SelectSingleNode("acSubCategory").InnerText.Trim(), xmlProduct.SelectSingleNode("acCategory").InnerText.Trim());
                        }
                    }
                }
            }
        }
    }
}
