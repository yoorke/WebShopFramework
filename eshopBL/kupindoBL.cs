using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eshopBE;
using eshopDL;
using System.Data;
using System.Xml;
using System.Web;
using System.Configuration;
using System.Text.RegularExpressions;

namespace eshopBL
{
    public class kupindoBL
    {
        public XmlDocument GetProducts()
        {
            DataTable products = new ProductDL().GetProductsForExport();
            XmlDocument xmlDoc = new XmlDocument();
            XmlTextWriter xmlWriter = new XmlTextWriter(HttpContext.Current.Server.MapPath("~/xml/products.xml"), Encoding.UTF8);
            xmlWriter.Formatting = Formatting.Indented;
            xmlWriter.WriteProcessingInstruction("xml", "version='1.0' encoding='UTF-8'");
            xmlWriter.WriteStartElement("root");
            xmlWriter.Close();

            xmlDoc.Load(HttpContext.Current.Server.MapPath("~/xml/products.xml"));

            XmlElement xmlRoot = xmlDoc.DocumentElement;

                XmlElement xmlInfo;
                xmlInfo = xmlDoc.CreateElement("Info");

                    XmlElement xmlVersion = xmlDoc.CreateElement("Version");
                    xmlVersion.InnerText = "2.0";
                    xmlInfo.AppendChild(xmlVersion);

                xmlRoot.AppendChild(xmlInfo);

                XmlElement xmlPredmeti = xmlDoc.CreateElement("Predmeti");

                    foreach(DataRow product in products.Rows)
                    {
                        XmlElement xmlProduct = xmlDoc.CreateElement("Predmet");
                            XmlElement xmlInternaSifra = xmlDoc.CreateElement("InternaSifra");
                            xmlInternaSifra.InnerText = product["code"].ToString();
                            xmlProduct.AppendChild(xmlInternaSifra);

                            XmlElement xmlNaziv = xmlDoc.CreateElement("Naziv");
                            xmlNaziv.InnerText = encodeText(product["brandName"].ToString() + " " + product["name"].ToString());
                            xmlProduct.AppendChild(xmlNaziv);

                            XmlElement xmlOpis = xmlDoc.CreateElement("Opis");
                            //xmlOpis.InnerText = product["description"].ToString();
                            xmlOpis.InnerText = new ProductBL().GetProductSpecificationText(int.Parse(product["productID"].ToString()));
                            xmlProduct.AppendChild(xmlOpis);
                
                            XmlElement xmlGarancija = xmlDoc.CreateElement("Garancija");
                            xmlGarancija.InnerText = product["garanty"].ToString() != string.Empty ? "1" : "0";
                            xmlProduct.AppendChild(xmlGarancija);

                            XmlElement xmlDuzinaGarancije = xmlDoc.CreateElement("DuzinaGarancije");
                            xmlDuzinaGarancije.InnerText = product["garanty"].ToString();
                            xmlProduct.AppendChild(xmlDuzinaGarancije);

                            XmlElement xmlNov = xmlDoc.CreateElement("Nov");
                            xmlNov.InnerText = "0";
                            xmlProduct.AppendChild(xmlNov);

                            XmlElement xmlCena = xmlDoc.CreateElement("Cena");
                            xmlCena.InnerText = product["webPrice"].ToString();
                            xmlProduct.AppendChild(xmlCena);

                            XmlElement xmlSlike = xmlDoc.CreateElement("Slike");
                                List<ProductImage> images = new ProductDL().GetProductImages(int.Parse(product["productID"].ToString()));
                                foreach(ProductImage productImage in images)
                                {
                                    string filename = productImage.ImageUrl.Substring(0, productImage.ImageUrl.LastIndexOf('.'));
                                    string extension = productImage.ImageUrl.Substring(productImage.ImageUrl.LastIndexOf('.'));

                                    XmlElement xmlUrl = xmlDoc.CreateElement("Url");
                                    xmlUrl.InnerText = ConfigurationManager.AppSettings["webShopUrl"] + new ProductBL().CreateImageDirectory(int.Parse(productImage.ImageUrl.Substring(0, productImage.ImageUrl.LastIndexOf('.')))) + filename + extension;
                                    xmlSlike.AppendChild(xmlUrl);
                                }
                            xmlProduct.AppendChild(xmlSlike);

                            XmlElement xmlCategory = xmlDoc.CreateElement("Kategorija");
                            xmlCategory.InnerText = product["category"].ToString();
                            xmlProduct.AppendChild(xmlCategory);

                            XmlElement xmlAttribute = xmlDoc.CreateElement("Karakteristike");
                            xmlAttribute.InnerText = product["brandID"].ToString();
                            xmlProduct.AppendChild(xmlAttribute);

                            XmlElement xmlLager = xmlDoc.CreateElement("Lager");
                            xmlLager.InnerText = "2";
                            xmlProduct.AppendChild(xmlLager);

                            XmlElement xmlAktivan = xmlDoc.CreateElement("Aktivan");
                            xmlAktivan.InnerText = bool.Parse(product["isActive"].ToString()) ? "1" : "0";
                            xmlProduct.AppendChild(xmlAktivan);

                            XmlElement xmlNacinPlacanja = xmlDoc.CreateElement("NaciniPlacanja");
                                XmlElement xmlLimundoCash = xmlDoc.CreateElement("LimundoCash");
                                xmlLimundoCash.InnerText = "1";
                                xmlNacinPlacanja.AppendChild(xmlLimundoCash);

                                XmlElement xmlSlanjePosleUplate = xmlDoc.CreateElement("SlanjePosleUplate");
                                xmlSlanjePosleUplate.InnerText = "1";
                                xmlNacinPlacanja.AppendChild(xmlSlanjePosleUplate);

                                XmlElement xmlSlanjePreUplate = xmlDoc.CreateElement("SlanjePreUplate");
                                xmlSlanjePreUplate.InnerText = "0";
                                xmlNacinPlacanja.AppendChild(xmlSlanjePreUplate);

                                XmlElement xmlSlanjePouzecem = xmlDoc.CreateElement("SlanjePouzecem");
                                xmlSlanjePouzecem.InnerText = "1";
                                xmlNacinPlacanja.AppendChild(xmlSlanjePouzecem);

                                XmlElement xmlLicnoPreuzimanje = xmlDoc.CreateElement("LicnoPreuzimanje");
                                xmlLicnoPreuzimanje.InnerText = "1";
                                xmlNacinPlacanja.AppendChild(xmlLicnoPreuzimanje);

                            
                            xmlProduct.AppendChild(xmlNacinPlacanja);

                            XmlElement xmlNacinSlanja = xmlDoc.CreateElement("NaciniSlanja");
                                XmlElement xmlNacinSlanjaLicnoPreuzimanje = xmlDoc.CreateElement("LicnoPreuzimanje");
                                xmlNacinSlanjaLicnoPreuzimanje.InnerText = "1";
                                xmlNacinSlanja.AppendChild(xmlNacinSlanjaLicnoPreuzimanje);

                                XmlElement xmlPosta = xmlDoc.CreateElement("Posta");
                                xmlPosta.InnerText = "1";
                                xmlNacinSlanja.AppendChild(xmlPosta);

                                XmlElement xmlAKS = xmlDoc.CreateElement("AKS");
                                xmlAKS.InnerText = "1";
                                xmlNacinSlanja.AppendChild(xmlAKS);

                                XmlElement xmlCityExpress = xmlDoc.CreateElement("CityExpress");
                                xmlCityExpress.InnerText = "1";
                                xmlNacinSlanja.AppendChild(xmlCityExpress);

                                XmlElement xmlPostExpress = xmlDoc.CreateElement("PostExpress");
                                xmlPostExpress.InnerText = "1";
                                xmlNacinSlanja.AppendChild(xmlPostExpress);

                                XmlElement xmlDailyExpress = xmlDoc.CreateElement("DailyExpress");
                                xmlDailyExpress.InnerText = "1";
                                xmlNacinSlanja.AppendChild(xmlDailyExpress);

                                XmlElement xmlOrganizovanTransport = xmlDoc.CreateElement("OrganizovaniTransport");
                                xmlOrganizovanTransport.InnerText = "1";
                                xmlNacinSlanja.AppendChild(xmlOrganizovanTransport);

                            xmlProduct.AppendChild(xmlNacinSlanja);
                    
                        xmlPredmeti.AppendChild(xmlProduct);
                    }

                xmlRoot.AppendChild(xmlPredmeti);

            xmlDoc.Save(HttpContext.Current.Server.MapPath("~/xml/products.xml"));

            return xmlDoc;
        }

        public DataTable GetCategories()
        {
            return new KupindoDL().GetCategories();
        }

        public DataTable GetMappedCategories()
        {
            return new KupindoDL().GetMappedCategories();
        }

        public int SaveMapping(int categoryID, int kupindoCategoryID)
        {
            return new KupindoDL().SaveMapping(categoryID, kupindoCategoryID);
        }

        private string encodeText(string text)
        {
            string[] chars = new string[] { "&", "<", ">", "\"", "'"};
            string[] replace = new string[] { "&amp;", "&lt;", "&gt;", "&quot;", "&apos;" };

            Regex regex;
            for(int i = 0; i < chars.Length; i++)
            {

                regex = new Regex(chars[i]);
                text = regex.Replace(text, replace[i]);
            }

            return text;
        }

        public DataTable GetKupindoCategoryForCategory(int categoryID)
        {
            return new KupindoDL().GetKupindoCategoryForCategory(categoryID);
        }

        public DataTable GetKupindoAttributes(int kupindoCategoryID)
        {
            return new KupindoDL().GetKupindoAttributes(kupindoCategoryID);
        }

        public int SaveKupindoAttributeForAttribute(int attributeID, int kupindoAttributeID)
        {
            return new KupindoDL().SaveKupindoAttributeForAttribute(attributeID, kupindoAttributeID);
        }

        public DataTable LoadSettings()
        {
            return new KupindoDL().LoadSettings();
        }

        public void SaveSettings(DataTable kupindoSettings)
        {
            new KupindoDL().SaveSettings(kupindoSettings);
        }
    }
}
