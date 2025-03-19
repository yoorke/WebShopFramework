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
                //routes.MapPageRoute("productPageWithoutCategory", "proizvodi/{*product}", "~/product.aspx", false, null, new RouteValueDictionary { { "product", new IsProductContraint() } });
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
                if (int.Parse(ConfigurationManager.AppSettings["accountPageVersion"]) == 1)
                    routes.MapPageRoute("moj-nalog", "moj-nalog", "~/account.aspx");
                else if (int.Parse(ConfigurationManager.AppSettings["accountPageVersion"]) == 2)
                    routes.MapPageRoute("moj-nalog", "moj-nalog", "~/account/user-dashboard.aspx");
                if(int.Parse(ConfigurationManager.AppSettings["accountPageVersion"]) == 1)
                    routes.MapPageRoute("izmena-sifre", "izmena-sifre", "~/passwordChange.aspx");
                routes.MapPageRoute("pretraga", "pretraga", "~/search.aspx");
                routes.MapPageRoute("porudzbina-uspesna", "porudzbina-uspesna", "~/orderSuccessful.aspx");
                routes.MapPageRoute("stampaProizvoda", "stampa-proizvoda/{productID}", "~/reports/reportTemplates/product/productPrint.aspx");
                routes.MapPageRoute("uporedi", "uporedi", "~/compare.aspx");
                routes.MapPageRoute("poredjenje-proizvoda", "poredjenje-proizvoda", "~/compare.aspx");
                routes.MapPageRoute("istorija-porudzbina", "istorija-porudzbina/{orderID}", "~/account/orderItems.aspx", false, null, null);
                if(int.Parse(ConfigurationManager.AppSettings["accountPageVersion"]) == 2)
                { 
                    routes.MapPageRoute("moj-profil", "moj-profil", "~/account/profile.aspx");
                    routes.MapPageRoute("istorija-porucivanja", "istorija-porucivanja", "~/account/orderHistory.aspx");
                    routes.MapPageRoute("izmena-sifre", "izmena-sifre", "~/account/passwordChange.aspx");
                }
                //if(bool.Parse(ConfigurationManager.AppSettings["createSitemapRoute"]))
                //routes.MapPageRoute("sitemap", "sitemap", "~/sitemapHandler.ashx");

                routes.MapPageRoute("cenovnik-dostave", "cenovnik-dostave", "~/cenovnikDostave.aspx");
                routes.MapPageRoute("placanje-uspesno", "placanje-uspesno", "~/paymentResponse.aspx");
                routes.MapPageRoute("placanje-neuspesno", "placanje-neuspesno", "~/paymentResponse.aspx");

                routes.MapPageRoute("saradnja", "saradnja", "~/saradnja.aspx");

                if (bool.Parse(ConfigurationManager.AppSettings["addSeparateGdeKupitiPage"]))
                {
                    routes.MapPageRoute("gde-kupiti", "gde-kupiti", "~/gdeKupiti.aspx");
                }

                if (bool.Parse(ConfigurationManager.AppSettings["enableIPSPayment"]))
                {
                    routes.MapPageRoute("ips-placanje", "ips-placanje", "~/IPSPayment.aspx");
                }


                foreach (CustomPage customPage in new CustomPageBL().GetCustomPages())
                {
                    if (bool.Parse(ConfigurationManager.AppSettings["addSeparateContactPage"]) && customPage.Url == "kontakt")
                    { 
                        continue;
                    }

                    if (customPage.IsActive)
                    { 
                        routes.MapPageRoute(customPage.Url, customPage.Url, "~/customPage.aspx", false, new RouteValueDictionary { { "url", customPage.Url } });
                    }
                }

                foreach (Promotion promotion in new PromotionBL().GetPromotions(false, null, null))
                {
                    routes.MapPageRoute(promotion.Name, "akcija/" + promotion.Url, "~/promotion.aspx", false, new RouteValueDictionary { { "url", promotion.Url } });
                }
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
