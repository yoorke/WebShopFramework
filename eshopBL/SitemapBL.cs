using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using eshopBE;
using System.Data;
using System.Web;
using System.Configuration;
using System.Globalization;
using System.Web.Configuration;
using System.IO;
using eshopDL;

namespace eshopBL
{
    public class SitemapBL
    {
        public void SaveSitemap(string type = "v1")
        {
            int siteMapValidity = int.Parse(ConfigurationManager.AppSettings["siteMapValidity"]);
            //DateTime siteMapCreatedDate = DateTime.ParseExact(ConfigurationManager.AppSettings["siteMapCreatedDate"], "d.M.yyyy", CultureInfo.InstalledUICulture);
            DateTime siteMapCreatedDate = getSitemapCreatedDate();

            //if(DateTime.Now > siteMapCreatedDate.AddDays(siteMapValidity))
            //{ 
                using (XmlTextWriter tw = new XmlTextWriter(HttpContext.Current.Server.MapPath("~/Web.sitemap"), Encoding.UTF8))
                {
                    tw.Formatting = Formatting.Indented;
                    tw.WriteProcessingInstruction("xml", "version='1.0' encoding='UTF-8'");
                    tw.WriteStartElement("siteMap", "http://schemas.microsoft.com/AspNet/SiteMap-File-1.0");
                }

                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(HttpContext.Current.Server.MapPath("~/Web.sitemap"));
                createXmlSitemap(xmlDoc, type);

                xmlDoc.Save(HttpContext.Current.Server.MapPath("~/Web.sitemap"));

                //Configuration configuration = WebConfigurationManager.OpenWebConfiguration("/");
                //configuration.AppSettings.Settings["siteMapCreatedDate"].Value = DateTime.UtcNow.ToString("d.M.yyyy");
                //configuration.Save();

                saveSitemapCreatedDate(DateTime.UtcNow);
            //}
        }

        private XmlDocument createXmlSitemap(XmlDocument xmlDoc, string type)
        {
            //XmlDocument xmlDoc = new XmlDocument();

            XmlElement root = xmlDoc.DocumentElement;
            

            XmlElement pocetna = createElement("siteMapNode", "/", "Početna", "Početna strana", xmlDoc);
            pocetna.AppendChild(createElement("siteMapNode", "/lista-zelja", "Lista želja", "Lista želja", xmlDoc));
            pocetna.AppendChild(createElement("siteMapNode", "/korpa", "Korpa", "Korpa", xmlDoc));
            pocetna.AppendChild(createElement("siteMapNode", "/porucivanje", "Poručivanje", "Poručivanje", xmlDoc));
            pocetna.AppendChild(createElement("siteMapNode", "/prijava", "Prijava", "Prijava", xmlDoc));
            pocetna.AppendChild(createElement("siteMapNode", "/moj-nalog", "Moj nalog", "Moja nalog", xmlDoc));
            pocetna.AppendChild(createElement("siteMapNode", "/registracija", "Registracija", "Registracija", xmlDoc));
            pocetna.AppendChild(createElement("siteMapNode", "/poredjenje-proizvoda", "Poređenje proizvoda", "Poređenje proizvoda", xmlDoc));
            pocetna.AppendChild(createElement("siteMapNode", "/resetovanje-sifre", "Reset šifre", "Reset korisničke šifre", xmlDoc));
            pocetna.AppendChild(createElement("siteMapNode", "/kreiranje-korisnicke-sifre", "Kreiranje šifre", "Kreiranje korisničke šifre", xmlDoc));
            pocetna.AppendChild(createElement("siteMapNode", "/izmena-sifre", "Izmena šifre", "Izmena korisničke šifre", xmlDoc));
            pocetna.AppendChild(createElement("siteMapNode", "/pretraga", "Pretraga", "Pretraga proizvoda", xmlDoc));
            pocetna.AppendChild(createElement("siteMapNode", "/porudzbina-uspesna", "Porudžbina uspešna", "Porudžbina uspešna", xmlDoc));
            pocetna.AppendChild(createElement("siteMapNode", "/uporedi", "Poređenje proizvoda", "Poređenje proizvoda", xmlDoc));
            pocetna.AppendChild(createElement("siteMapNode", "/poređenje-proizvoda", "Poređenje proizvoda", "Poređenje proizvoda", xmlDoc));
            pocetna.AppendChild(createElement("siteMapNode", "/istorija-porudzbina", "Istorija porudžbina", "Istorija porudžbina", xmlDoc));
            pocetna.AppendChild(createElement("siteMapNode", "/cenovnik-dostave", "Cenovnik dostava", "Cenovnik dostave", xmlDoc));
            pocetna.AppendChild(createElement("siteMapNode", "/saradnja", "Saradnja", "Saradnja", xmlDoc));
            pocetna.AppendChild(createElement("siteMapNode", "/gde-kupiti", "Gde kupiti", "Gde kupiti", xmlDoc));

            if (int.Parse(ConfigurationManager.AppSettings["accountPageVersion"]) == 2)
            {
                pocetna.AppendChild(createElement("siteMapNode", "/moj-profil", "Profil", "Korisnički profil", xmlDoc));
                pocetna.AppendChild(createElement("siteMapNode", "/istorija-porucivanja", "Istorija poručivanja", "Istorija poručivanja", xmlDoc));
            }

            if (bool.Parse(ConfigurationManager.AppSettings["addSeparateContactPage"]))
            {
                pocetna.AppendChild(createElement("siteMapNode", "/kontakt", "Kontakt", "Kontakt", xmlDoc));
            }

            if (bool.Parse(ConfigurationManager.AppSettings["hasRetails"]))
            {
                pocetna.AppendChild(createElement("siteMapNode", "/prodajna-mesta", "Prodajna mesta", "Prodajna mesta", xmlDoc));
            }

            if (bool.Parse(ConfigurationManager.AppSettings["enableCardPayment"]))
            {
                pocetna.AppendChild(createElement("siteMapNode", "/placanje-uspesno", "Plaćanje uspešno", "Plaćanje karticom uspešno realizovano", xmlDoc));
                pocetna.AppendChild(createElement("siteMapNode", "/placanje-neuspesno", "Plaćanje neuspešno", "Plaćanje karticom nije uspešno realizovano", xmlDoc));
            }

            foreach (CustomPage customPage in new CustomPageBL().GetCustomPages())
            {
                if (bool.Parse(ConfigurationManager.AppSettings["addSeparateContactPage"]) && customPage.Url == "kontakt")
                {
                    continue;
                }

                if (customPage.IsActive)
                {
                    pocetna.AppendChild(createElement("siteMapNode", "/" + customPage.Url, customPage.Title, customPage.Title, xmlDoc));
                }
            }

            if(type.Equals("v1"))
            {
                List<Category> categories = getCategoriesV1();

                foreach (Category category in categories)
                {
                    //if(int.Parse(categoryRow["parentID"].ToString()) != 0)
                    //{ 
                    pocetna.AppendChild(createProductsSiteMapV1(category, xmlDoc));
                    //}
                }
            }
            else if(type.Equals("v2"))
            {
                List<CategoryView> categories = getCategoriesV2();

                foreach (CategoryView category in categories)
                {
                    List<Category> subCategories = new CategoryBL().GetAllSubCategories(category.CategoryID, false);
                    XmlElement categoryElement = createElement("siteMapNode", category.FullUrl, category.Name, category.Name, xmlDoc);
                    
                    List<Product> products = null;

                    XmlElement subCategoryElement = null;

                    if (subCategories == null || subCategories.Count == 0)
                    {
                        products = new ProductBL().GetProductsForSitemap(category.CategoryID);

                        if (products != null)
                        {
                            foreach (Product product in products)
                            {
                                categoryElement.AppendChild(createElement("siteMapNode", product.Url, $"{product.Name} {product.ListDescription}", $"{product.Name} {product.ListDescription}", xmlDoc));
                            }
                        }
                    }
                    else
                    {
                        foreach (Category subCategory in subCategories)
                        {
                            subCategoryElement = createElement("siteMapNode", subCategory.Url, subCategory.Name, subCategory.Name, xmlDoc);
                            products = new ProductBL().GetProductsForSitemap(subCategory.CategoryID);

                            if (products != null)
                            {
                                foreach (Product product in products)
                                {
                                    subCategoryElement.AppendChild(createElement("siteMapNode", product.Url, $"{product.Name} {product.ListDescription}", $"{product.Name} {product.ListDescription}", xmlDoc));
                                }
                            }

                            pocetna.AppendChild(subCategoryElement);
                        }
                    }

                    pocetna.AppendChild(categoryElement);
                }
            }

            root.AppendChild(pocetna);

            return xmlDoc;
        }

        private List<Category> getCategoriesV1()
        {
            return new CategoryBL().GetNestedCategoriesList(false);
        }

        private List<CategoryView> getCategoriesV2()
        {
            return new CategoryViewBL().GetCategoriesForFirstPage();
        }

        private XmlElement createElement(string name, string url, string title, string description, XmlDocument xmlDoc)
        {
            XmlElement element = xmlDoc.CreateElement(name);
            element.SetAttribute("url", url);
            element.SetAttribute("title", title);
            element.SetAttribute("description", description);

            return element;
        }

        private XmlElement createProductsSiteMapV1(Category category, XmlDocument xmlDoc)
        {
            //List<Product> products = new ProductBL().GetProductsForCategory(category.CategoryID, true, true);
            List<Product> products = new ProductBL().GetProductsForSitemap(category.CategoryID);
            XmlElement categoryElement = createElement("siteMapNode", category.Url, category.Name, category.Name, xmlDoc);
            if (products != null)
            {
                foreach (Product product in products)
                {
                    categoryElement.AppendChild(createElement("siteMapNode", product.Url, product.Name, product.Name, xmlDoc));
                }
            }

            if(category.SubCategory != null)
            {
                foreach (Category subCategory in category.SubCategory)
                    categoryElement.AppendChild(createProductsSiteMapV1(subCategory, xmlDoc));
            }

            return categoryElement;
        }

        private DateTime getSitemapCreatedDate()
        {
            DateTime sitemapCreatedDate = DateTime.Now;
            using (TextReader tr = new StreamReader(HttpContext.Current.Server.MapPath("~/sitemap.config")))
            {
                sitemapCreatedDate = DateTime.ParseExact(tr.ReadLine(), "d.M.yyyy", CultureInfo.InvariantCulture);
            }

            return sitemapCreatedDate;
        }

        private void saveSitemapCreatedDate(DateTime sitemapCreatedDate)
        {
            using (TextWriter tw = new StreamWriter(HttpContext.Current.Server.MapPath("~/sitemap.config"), false))
            {
                tw.Write(sitemapCreatedDate.ToString("d.M.yyyy"));
            }
        }
    }
}
