using eshopBE;
using eshopDL;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eshopBL
{
    public class ProductVariantBL
    {
        public void Save(ProductVariant productVariant)
        {
            new ProductVariantDL().Save(productVariant);
        }

        public DataTable GetProductVariants(int productID)
        {
            return new ProductVariantDL().GetProductVariants(productID);
        }

        public void DeleteProductVariant(int id)
        {
            new ProductVariantDL().DeleteProductVariant(id);
        }

        public void SetIsInStock(int id, bool isInStock)
        {
            new ProductVariantDL().SetIsInStock(id, isInStock);
        }

        public List<ProductVariantAttributeValue> GetAttributeValues(int productID)
        {
            return new ProductVariantDL().GetAttributeValues(productID);
        }
    }
}
