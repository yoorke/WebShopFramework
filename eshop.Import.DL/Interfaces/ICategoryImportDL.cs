using eshop.Import.BE;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eshop.Import.DL.Interfaces
{
    public interface ICategoryImportDL
    {
        List<CategoryImport> GetCategories(string supplierCode, string parentCategory);
        //List<CategoryImport> GetSupplierCategories(string supplierCode);
        //CategoryImport GetSupplierCategory(int supplierCategoryID);
        //List<CategoryImport> GetNewSupplierCategories(string supplierCode);
        CategoryImport GetSupplierCategoryForCategoryID(int categoryID);
        List<CategoryImport> GetSupplierCategoriesForCategoryID(int categoryID);
        void SaveSupplierCategory(string category, string parentCategory);
        //void SaveSelectedCategory(List<CategoryImport> categories);
        void SaveSupplierCategoryForCategory(int categoryID, int supplierCategoryID, bool isCategory);
        void DeleteCategorySupplierCategory(int categoryID);
    }
}
