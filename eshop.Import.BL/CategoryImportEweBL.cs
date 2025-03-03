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
    public class CategoryImportEweBL : BaseCategoryImportBL
    {
        private ICategoryImportDL _categoryImportDL = new CategoryImportEweDL();
        private IProductImportDL _productImportDL = new ProductImportEweDL();

        public CategoryImportEweBL() : base(new CategoryImportEweDL(), "ewe")
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
                    if(xmlProduct.SelectNodes("category") != null)
                    {
                        _categoryImportDL.SaveSupplierCategory(xmlProduct.SelectSingleNode("category").InnerText.Trim(), null);

                        if(xmlProduct.SelectSingleNode("subcategory") != null)
                        {
                            _categoryImportDL.SaveSupplierCategory(xmlProduct.SelectSingleNode("subcategory").InnerText.Trim(), xmlProduct.SelectSingleNode("category").InnerText.Trim());
                        }
                    }
                }
            }
        }
    }
}
