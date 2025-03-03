using eshop.Import.BE;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eshop.Import.BL.Interfaces
{
    public interface ICategoryImportBL
    {
        void UpdateSupplierCategories();
        List<CategoryImport> GetCategories(string parentCategory);
        CategoryImport GetSupplierCategoryForCategoryID(int categoryID);
        List<CategoryImport> GetSupplierCategoriesForCategoryID(int categoryID);
        void SaveSupplierCategoryForCategory(int categoryID, int supplierCategoryID, bool isCategory);
        void DeleteCategorySupplierCategory(int categoryID);
    }
}
