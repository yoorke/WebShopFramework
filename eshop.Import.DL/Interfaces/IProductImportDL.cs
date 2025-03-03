using eshop.Import.BE;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace eshop.Import.DL.Interfaces
{
    public interface IProductImportDL
    {
        XmlDocument DownloadProducts(string category, string subCategory, GetParameter getParameter);
        List<ProductImport> GetProducts(string category, List<string> subCategories, string manufacturer);
        ProductImport GetProductBySupplierCode(string supplierCore, string supplierProductCode);
        
        void SaveProducts(List<ProductImport> products, string category);

        List<ManufacturerImport> GetManufacturers(List<string> subCategories);
    }
}
