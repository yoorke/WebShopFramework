using eshop.Import.BE;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eshop.Import.BL.Interfaces
{
    public interface IProductImportBL
    {
        //void DownloadProducts(GetParameter getParameter);
        List<ProductImport> ParseProducts(string category, List<string> subCategories, GetParameter getParameter, bool updatePriceAndStock = false, bool saveToDatabase = false);
        int UpdatePriceAndStock(string category, List<string> subCategories);
        
        List<ProductImport> GetProducts(string category, List<string> subCategories, string manufacturer);

        void SaveProduct(string supplierCode, bool isApproved, bool isActive, int categoryID);
        void SaveProducts(List<ProductImport> products);
        int UpdateProducts(List<ProductImport> products);

        List<ManufacturerImport> GetManufacturers(List<string> subCategories, bool addSelectAll = true);
    }
}
