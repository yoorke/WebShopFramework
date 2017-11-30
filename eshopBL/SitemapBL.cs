using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using eshopBE;
using System.Data;
using System.Web;
using System.Configuration;

namespace eshopBL
{
    public class SitemapBL
    {
        public void SaveSitemap()
        {
            using (XmlTextWriter tw = new XmlTextWriter(HttpContext.Current.Server.MapPath("~/Web.sitemap"), Encoding.UTF8))
            {
                tw.Formatting = Formatting.Indented;
                tw.WriteProcessingInstruction("xml", "version='1.0' encoding='UTF-8'");
                tw.WriteStartElement("siteMap", "http://schemas.microsoft.com/AspNet/SiteMap-File-1.0");
            }

            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(HttpContext.Current.Server.MapPath("~/Web.sitemap"));
            CreateXmlSitemap(xmlDoc);

            xmlDoc.Save(HttpContext.Current.Server.MapPath("~/Web.sitemap"));
        }

        public XmlDocument CreateXmlSitemap(XmlDocument xmlDoc)
        {
            //XmlDocument xmlDoc = new XmlDocument();

            XmlElement root = xmlDoc.DocumentElement;
            

            XmlElement pocetna = createElement("siteMapNode", "/", "Početna", "Početna strana", xmlDoc);
            pocetna.AppendChild(createElement("siteMapNode", "/lista-zelja", "Lista želja", "Lista želja", xmlDoc));
            pocetna.AppendChild(createElement("siteMapNode", "/moj-nalog", "Moj nalog", "Moja nalog", xmlDoc));
            pocetna.AppendChild(createElement("siteMapNode", "/korpa", "Korpa", "Korpa", xmlDoc));
            pocetna.AppendChild(createElement("siteMapNode", "/porucivanje", "Poručivanje", "Poručivanje", xmlDoc));
            pocetna.AppendChild(createElement("siteMapNode", "/kontakt", "Kontakt", "Kontakt", xmlDoc));
            pocetna.AppendChild(createElement("siteMapNode", "/prijava", "Prijava", "Prijava", xmlDoc));
            pocetna.AppendChild(createElement("siteMapNode", "/registracija", "Registracija", "Registracija", xmlDoc));

            if (bool.Parse(ConfigurationManager.AppSettings["hasRetails"]))
                pocetna.AppendChild(createElement("siteMapNode", "/prodajna-mesta", "Prodajna mesta", "Prodajna mesta", xmlDoc));

            foreach(CustomPage customPage in new CustomPageBL().GetCustomPages())
                pocetna.AppendChild(createElement("siteMapNode", "/" + customPage.Url, customPage.Title, customPage.Title, xmlDoc));

            List<Category> categories = new CategoryBL().GetNestedCategoriesList(false);
            foreach (Category category in categories)
            {
                //if(int.Parse(categoryRow["parentID"].ToString()) != 0)
                //{ 
                    

                    

                    pocetna.AppendChild(createProductsSiteMap(category, xmlDoc));
                //}
            }

            root.AppendChild(pocetna);

            return xmlDoc;
        }

        private XmlElement createElement(string name, string url, string title, string description, XmlDocument xmlDoc)
        {
            XmlElement element = xmlDoc.CreateElement(name);
            element.SetAttribute("url", url);
            element.SetAttribute("title", title);
            element.SetAttribute("description", description);

            return element;
        }

        private XmlElement createProductsSiteMap(Category category, XmlDocument xmlDoc)
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
                    categoryElement.AppendChild(createProductsSiteMap(subCategory, xmlDoc));
            }

            return categoryElement;
        }
    }
}
