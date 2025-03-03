using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Configuration;
using eshopBE;
using System.Data;

namespace eshopBL
{
    public class WebSitemapBL
    {
        public XmlDocument CreateSitemap()
        {
            string webShopUrl = ConfigurationManager.AppSettings["webShopUrl"];

            XmlDocument xmlDoc = new XmlDocument();

            XmlDeclaration xmlDeclaration = xmlDoc.CreateXmlDeclaration("1.0", "UTF-8", "yes");


            XmlElement root = xmlDoc.CreateElement("urlset");
            root.SetAttribute("xmlns", "http://www.sitemaps.org/schemas/sitemap/0.9");
            xmlDoc.AppendChild(root);
            xmlDoc.InsertBefore(xmlDeclaration, root);


            root.AppendChild(createElement(webShopUrl, xmlDoc));
            root.AppendChild(createElement(webShopUrl + "/lista-zelja", xmlDoc));
            root.AppendChild(createElement(webShopUrl + "/moj-nalog", xmlDoc));
            root.AppendChild(createElement(webShopUrl + "/korpa", xmlDoc));
            root.AppendChild(createElement(webShopUrl + "/porucivanje", xmlDoc));
            root.AppendChild(createElement(webShopUrl + "/kontakt", xmlDoc));
            root.AppendChild(createElement(webShopUrl + "/prijava", xmlDoc));
            root.AppendChild(createElement(webShopUrl + "/registracija", xmlDoc));
            root.AppendChild(createElement(webShopUrl + "/poredjenje-proizvoda", xmlDoc));
            if(bool.Parse(ConfigurationManager.AppSettings["addSeparateContactPage"]))
                root.AppendChild(createElement(webShopUrl + "/kontakt", xmlDoc));

            if(int.Parse(ConfigurationManager.AppSettings["accountPageVersion"]) == 2)
            {
                root.AppendChild(createElement(webShopUrl + "/istorija-porudzbina", xmlDoc));
                root.AppendChild(createElement(webShopUrl + "/moj-profil", xmlDoc));
            }

            if (bool.Parse(ConfigurationManager.AppSettings["hasRetails"]))
                root.AppendChild(createElement(webShopUrl + "/prodajna-mesta", xmlDoc));

            foreach (CustomPage customPage in new CustomPageBL().GetCustomPages())
                root.AppendChild(createElement(webShopUrl + "/" + customPage.Url, xmlDoc));

            DataTable categories = new CategoryBL().GetCategories("categoryID", false);
            foreach(DataRow categoryRow in categories.Rows)
            {
                string categoryUrl = bool.Parse(ConfigurationManager.AppSettings["includeParentUrlInCategoryUrl"]) ? categoryRow["fullUrl"].ToString() : categoryRow["url"].ToString();
                if (!categoryUrl.Contains("proizvodi"))
                    categoryUrl = "/proizvodi/" + categoryUrl;
                root.AppendChild(createElement(webShopUrl + (!categoryUrl.ToString().StartsWith("/") ? "/" : string.Empty) + categoryUrl.ToString(), xmlDoc));
                foreach (Product product in new ProductBL().GetProductsForSitemap(int.Parse(categoryRow["categoryID"].ToString())))
                    root.AppendChild(createElement(webShopUrl + product.Url, xmlDoc));
            }

            return xmlDoc;
        }

        private XmlElement createElement(string url, XmlDocument xmlDoc)
        {
            XmlElement element = xmlDoc.CreateElement("url");
            XmlElement urlElement = xmlDoc.CreateElement("loc");
            urlElement.InnerText = url;
            element.AppendChild(urlElement);

            return element;
        }

        
    }
}
