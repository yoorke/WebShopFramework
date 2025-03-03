using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace eshopDL.Interfaces
{
    public interface IProductImportDL
    {
        XmlDocument GetXml(string category, string subCategory, bool getImages, bool getAttributes);
        DataTable GetImportCategories(int parentCategoryID, int categoryID);
        int SaveCategory(string category, string parentCategory);
        int SaveSelected(string[] categoryIDs, string[] selected);
        DataTable GetNewCategories();
        int GetImportCategoryForCategory(int categoryID);
        int SaveImportCategoryForCategory(int categoryID, int importCategoryID, bool isCategory);
        int DeleteImportCategories();
        int DeleteCategoryImportCategory(int categoryID);
        int SaveProducts(DataTable products, string importCategory, int categoryID, int importCategoryID);
        DataTable GetProducts(string category, string[] subCategories);
        DataTable GetProductBySupplierCode(string supplierCode);
        string[] GetImportCategory(int importCategoryID);
    }
}
