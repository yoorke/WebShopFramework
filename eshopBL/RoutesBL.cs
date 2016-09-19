using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Routing;
using eshopBE;

namespace eshopBL
{
    public class RoutesBL
    {
        public void RegisterRoutes()
        {
            RouteCollection routes = RouteTable.Routes;
            using (routes.GetWriteLock())
            {
                routes.Clear();
                routes.MapPageRoute("category", "proizvodi/{category}", "~/products.aspx");
                routes.MapPageRoute("product", "proizvodi/{category}/{product}", "~/product.aspx");
                routes.MapPageRoute("lista-zelja", "lista-zelja", "~/wishList.aspx");
                routes.MapPageRoute("korpa", "korpa", "~/cart.aspx");
                routes.MapPageRoute("porucivanje", "porucivanje", "~/checkout.aspx");
                routes.MapPageRoute("registracija", "registracija", "~/registration.aspx");
                routes.MapPageRoute("prijava", "prijava", "~/login.aspx");
                routes.MapPageRoute("resetovanje-sifre", "resetovanje-sifre", "~/passwordResetRequest.aspx");
                routes.MapPageRoute("kreiranje-korisnicke-sifre", "kreiranje-korisnicke-sifre", "~/passwordReset.aspx");
                routes.MapPageRoute("kontakt", "kontakt", "~/kontakt.aspx");
                routes.MapPageRoute("moj-nalog", "moj-nalog", "~/account.aspx");
                routes.MapPageRoute("izmena-sifre", "izmena-sifre", "~/passwordChange.aspx");
                routes.MapPageRoute("pretraga", "pretraga", "~/search.aspx");


                foreach (CustomPage customPage in new CustomPageBL().GetCustomPages())
                {
                    routes.MapPageRoute(customPage.Url, customPage.Url, "~/customPage.aspx", false, new RouteValueDictionary { { "url", customPage.Url } });
                }

                foreach (Promotion promotion in new PromotionBL().GetPromotions(false, null, null))
                    routes.MapPageRoute(promotion.Name, "akcija/" + promotion.Url, "~/promotion.aspx", false, new RouteValueDictionary { { "url", promotion.Url } });
            }
        }
    }
}
