using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using eshopBE;
using eshopDL;
using eshopUtilities;

namespace eshopBL
{
    public class WishListBL
    {
        public int SaveProductToWishList(int userID, int productID)
        {
            return new WishListDL().SaveProdutToWishList(userID, productID);
        }

        public List<Product> GetWishListProducts(int userID)
        {
            return new WishListDL().GetWishListProducts(userID);
        }

        public int DeleteFromWishList(int userID, int productID)
        {
            return new WishListDL().DeleteFromWishList(userID, productID);
        }
    }
}
