using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Routing;
using eshopBE;
using System.Configuration;
using System.Web;
using System.Text.RegularExpressions;

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

                routes.MapPageRoute("productPage", "proizvodi/{category}/{*product}", "~/product.aspx", false, null, new RouteValueDictionary { { "product", new IsProductContraint() } });
                routes.MapPageRoute("category", "proizvodi/{*category}", "~/products.aspx");
                routes.MapPageRoute("product", "proizvodi/{category}/{product}", "~/product.aspx");
                routes.MapPageRoute("lista-zelja", "lista-zelja", "~/wishList.aspx");
                routes.MapPageRoute("korpa", "korpa", "~/cart.aspx");
                routes.MapPageRoute("porucivanje", "porucivanje", "~/checkout.aspx");
                routes.MapPageRoute("registracija", "registracija", "~/registration.aspx");
                routes.MapPageRoute("prijava", "prijava", "~/login.aspx");
                routes.MapPageRoute("resetovanje-sifre", "resetovanje-sifre", "~/passwordResetRequest.aspx");
                routes.MapPageRoute("kreiranje-korisnicke-sifre", "kreiranje-korisnicke-sifre", "~/passwordReset.aspx");
                if(bool.Parse(ConfigurationManager.AppSettings["addSeparateContactPage"]))
                    routes.MapPageRoute("kontakt", "kontakt", "~/kontakt.aspx");
                routes.MapPageRoute("moj-nalog", "moj-nalog", "~/account.aspx");
                routes.MapPageRoute("izmena-sifre", "izmena-sifre", "~/passwordChange.aspx");
                routes.MapPageRoute("pretraga", "pretraga", "~/search.aspx");
                routes.MapPageRoute("porudzbina-uspesna", "porudzbina-uspesna", "~/orderSuccessful.aspx");
                routes.MapPageRoute("stampaProizvoda", "stampa-proizvoda/{productID}", "~/reports/reportTemplates/product/productPrint.aspx");
                routes.MapPageRoute("uporedi", "uporedi", "~/compare.aspx");


                foreach (CustomPage customPage in new CustomPageBL().GetCustomPages())
                {
                    routes.MapPageRoute(customPage.Url, customPage.Url, "~/customPage.aspx", false, new RouteValueDictionary { { "url", customPage.Url } });
                }

                foreach (Promotion promotion in new PromotionBL().GetPromotions(false, null, null))
                    routes.MapPageRoute(promotion.Name, "akcija/" + promotion.Url, "~/promotion.aspx", false, new RouteValueDictionary { { "url", promotion.Url } });
            }
        }
    }

    public class IsProductContraint : IRouteConstraint
    {
        public bool Match(HttpContextBase httpContext, Route route, string parameterName, RouteValueDictionary values, RouteDirection routeDirection)
        {
            if (values["product"] == null)
                return false;

            Regex regex = new Regex(@"-(\d+)$");

            return (regex.Match(values["product"].ToString())).Success;
        }
    }
}
