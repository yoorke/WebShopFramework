using eshopBE;
using eshopDL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eshopBL
{
    public class ProductViewBL
    {
        public List<ProductFPView> GetProducts(string categoryUrl, List<string> brands, List<AttributeValue> attributeValues, string sortName, string priceFrom, string priceTo, bool includeChildrenCategories = false)
        {
            Category category = new CategoryBL().GetCategoryByUrl(categoryUrl);

            return new ProductViewDL().GetProducts(category.CategoryID, brands, attributeValues, getSort(sortName), getPrice(priceFrom), getPrice(priceTo), includeChildrenCategories || category.ShowProductsFromSubCategories);
        }

        private string getSort(string sortName)
        {
            string sort = " product.name";
            switch(sortName)
            {
                case "name": { sort = " product.name"; break; }
                case "priceDesc": { sort = " product.price DESC"; break; }
                case "priceAsc": { sort = " product.price"; break; }
                case "sortIndex": { sort = " sortIndex"; break; }
            }
            return sort;
        }

        private double getPrice(string priceString)
        {
            double price = 0;
            double.TryParse(priceString, out price);

            return price;
        }

        public List<ProductFPView> GetProductsForFirstPage(int categoryID, int brandID, int numberOfProducts, ProductOrderByType orderBy, int supplierID = -1)
        {
            return new ProductViewDL().GetProductsForFirstPage(categoryID, brandID, numberOfProducts, orderBy, supplierID);
        }

    }
}
