using eshop.Import.BE;
using eshop.Import.BL.Interfaces;
using eshop.Import.DL.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eshop.Import.BL.AbstractClasses
{
    public abstract class BaseCategoryImportBL : ICategoryImportBL
    {
        private ICategoryImportDL _categoryImportDL;
        private string _supplierCode;

        public BaseCategoryImportBL(ICategoryImportDL categoryImportDL, string supplierCode)
        {
            _categoryImportDL = categoryImportDL;
            _supplierCode = supplierCode;
        }

        public abstract void UpdateSupplierCategories();

        public List<CategoryImport> GetCategories(string parentCategory)
        {
            return _categoryImportDL.GetCategories(_supplierCode, parentCategory);
        }

        public CategoryImport GetSupplierCategoryForCategoryID(int categoryID)
        {
            return _categoryImportDL.GetSupplierCategoryForCategoryID(categoryID);
        }

        public List<CategoryImport> GetSupplierCategoriesForCategoryID(int categoryID)
        {
            return _categoryImportDL.GetSupplierCategoriesForCategoryID(categoryID);
        }

        public void SaveSupplierCategoryForCategory(int categoryID, int supplierCategoryID, bool isCategory)
        {
            _categoryImportDL.SaveSupplierCategoryForCategory(categoryID, supplierCategoryID, isCategory);
        }

        public void DeleteCategorySupplierCategory(int categoryID)
        {
            _categoryImportDL.DeleteCategorySupplierCategory(categoryID);
        }
    }
}