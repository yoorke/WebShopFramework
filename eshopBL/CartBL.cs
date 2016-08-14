using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using eshopDL;
using eshopBE;
using System.Web;

namespace eshopBL
{
    public class CartBL
    {
        public int AddProductToCart(int productID, string cartID, double quantity, double productPrice, double userPrice)
        {
            CartDL cartDL = new CartDL();
            int status = cartDL.AddProductToCart(productID, cartID, quantity, productPrice, userPrice);
            ApplyCoupon(cartID);
            return status;
        }

        public int DeleteProductFromCart(int productID, string cartID)
        {
            CartDL cartDL = new CartDL();
            return cartDL.DeleteProductFromCart(productID, cartID);
        }

        public DataTable GetProducts(string cartID)
        {
            CartDL cartDL = new CartDL();
            return cartDL.GetProducts(cartID);
        }

        public int GetProductsCount(string cartID)
        {
            CartDL cartDL = new CartDL();
            return cartDL.GetProductsCount(cartID);
        }

        public int UpdateCartProduct(string cartID, int productID, double quantity, double productPrice, double userPrice, int couponID)
        {
            CartDL cartDL = new CartDL();
            return cartDL.UpdateCartProduct(cartID, productID, quantity, productPrice, userPrice, couponID);
        }

        public double GetCartDiscount(string cartID)
        {
            CartDL cartDL = new CartDL();
            return cartDL.GetCartDiscount(cartID);
        }

        public int SaveCartCoupon(string cartID, int couponID)
        {
            CartDL cartDL = new CartDL();
            return cartDL.SaveCartCoupon(cartID, couponID);
        }

        public double GetTotal(string cartID)
        {
            DataTable products = GetProducts(cartID);
            double total = 0;

            for (int i = 0; i < products.Rows.Count; i++)
                total += double.Parse(products.Rows[i]["userPrice"].ToString()) * double.Parse(products.Rows[i]["quantity"].ToString());
            return total;
        }

        public int GetCartCoupon(string cartID)
        {
            CartDL cartDL = new CartDL();
            return cartDL.GetCartCoupon(cartID);
        }

        public void RemoveCoupon(string cartID)
        {
            new CartDL().RemoveCoupon(cartID);
        }

        public void ApplyCoupon(string cartID)
        {
            Coupon coupon = new CouponBL().GetCoupon(GetCartCoupon(cartID));
            if (coupon != null)
            {
                DataTable cart = GetProducts(cartID);
                if (coupon.Objects != null && coupon.Objects.Count > 0)
                {
                    for (int i = 0; i < cart.Rows.Count; i++)
                    {
                        Product product = new ProductBL().GetProduct(int.Parse(cart.Rows[i]["productID"].ToString()), string.Empty, false);
                        bool[] couponObjectStatus = new bool[coupon.Objects.Count];
                        for (int j = 0; j < coupon.Objects.Count; j++)
                        {
                            //category
                            if (coupon.Objects[j].ObjectTypeID == 1)
                            {
                                List<Category> categories = new CategoryBL().GetAllSubCategories(coupon.Objects[j].ObjectID);
                                foreach(Category category in categories)
                                    if(category.CategoryID == product.Categories[0].CategoryID)
                                    { 
                                        couponObjectStatus[j] = true;
                                        break;
                                    }
                            }
                            //brand
                            if (coupon.Objects[j].ObjectTypeID == 2 && product.Brand.BrandID == coupon.Objects[j].ObjectID)
                                couponObjectStatus[j] = true;
                            //product
                            if (coupon.Objects[j].ObjectTypeID == 3 && product.ProductID == coupon.Objects[j].ObjectID)
                                couponObjectStatus[j] = true;
                        }
                        bool status = true;
                        foreach (bool objectStatus in couponObjectStatus)
                            status = status && objectStatus;

                        if (status)
                        {
                            double productPrice = double.Parse(cart.Rows[i]["productPrice"].ToString());
                            double discountPrice = coupon.CouponType.CouponTypeID == 1 ? productPrice * (1 - coupon.Discount / 100) : productPrice - coupon.Discount;
                            UpdateCartProduct(cartID, product.ProductID, double.Parse(cart.Rows[i]["quantity"].ToString()), double.Parse(cart.Rows[i]["productPrice"].ToString()), discountPrice, coupon.CouponID);
                        }
                    }
                }
            }
        }

        public void ClearItems(string cartID)
        {
            DataTable products = GetProducts(cartID);
            for (int i = 0; i < products.Rows.Count; i++)
                DeleteProductFromCart(int.Parse(products.Rows[i]["productID"].ToString()), cartID);          
        }
    }
}
