using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eshopBE;
using System.Xml;
using eshopDL;
using System.IO;
using System.Web;
using System.Net;
using System.Data;
using System.Xml.Xsl;
using System.Web;
using System.Drawing;
using eshopUtilities;

namespace eshopBL
{
    public class EweBL
    {
        private int getAttributeID(List<eshopBE.Attribute> attributes, string attributeName)
        {
            int attributeID = 0;
            if (attributes != null)
            {
                for (int i = 0; i < attributes.Count; i++)
                {
                    if (attributes[i].Name == attributeName)
                    {
                        attributeID = attributes[i].AttributeID;
                        break;
                    }
                }
            }
            return attributeID;
        }

        public int getAttributeValueID(int attributeID, string value)
        {
            AttributeBL attributeBL = new AttributeBL();
            List<AttributeValue> attributeValues = attributeBL.GetAttributeValues(attributeID);
            int attributeValueID = 0;
            bool exists = false;

            if (attributeValues != null)
            {
                for (int i = 0; i < attributeValues.Count; i++)
                    if (attributeValues[i].Value == value)
                    {
                        attributeValueID = attributeValues[i].AttributeValueID;
                        exists = true;
                        break;
                    }
            }

            if (!exists)
            {
                AttributeValue attributeValue = new AttributeValue(-1, value, attributeID, 0, string.Empty, 0);
                attributeValueID = attributeBL.SaveAttributeValue(attributeValue, false);
            }
            return attributeValueID;
        }

        private int addAttribute(int categoryID, string attributeName)
        {
            AttributeBL attributeBL = new AttributeBL();
            eshopBE.Attribute attribute = new eshopBE.Attribute();
            attribute.Name = attributeName;
            attribute.Filter = false;
            int attributeID = attributeBL.SaveAttribute(attribute);

            CategoryBL categoryBL = new CategoryBL();
            attributeBL.SaveAttributeForCategory(categoryID, attributeID);

            return attributeID;
        }

        public string parseProducts(string getCategory, string[] getSubcategories, bool getImages, bool getAttributes, string categoryName, int categoryID, bool overwrite, string[] codes, bool isActive, bool isApproved)
        {
            try
            {
                ErrorLog.LogMessage("parsing products");
                EweDL eweDL = new EweDL();
                List<eshopBE.Attribute> attributes = null;
                ProductBL productBL = new ProductBL();
                AttributeBL attributeBL = new AttributeBL();
                attributes = attributeBL.GetAttributesForCategory(categoryID);
                CategoryDL categoryDL = new CategoryDL();
                Category category = categoryDL.GetCategory(categoryID);
                //productBL.SetInStock(1, false, categoryID);
                int newProducts = 0;
                int updatedProducts = 0;
                bool isNew = false;



                /*List<string> subcategory = new List<string>();
                string categoryString=string.Empty;
                foreach (XmlNode xmlNode in nodeList)
                    foreach (XmlNode xmlChildNode in xmlNode.ChildNodes)
                    {
                        if (xmlChildNode.Name == "category")
                        {
                            categoryString = xmlChildNode.InnerText;
                        }
                        if (xmlChildNode.Name == "subcategory")
                        {
                            categoryString += "-" + xmlChildNode.InnerText;
                            subcategory.Add(categoryString);
                        }
                    }
                string prethodni = string.Empty;
                using (StreamWriter sw = new StreamWriter(Server.MapPath("~") + "categories.txt"))
                {
                    foreach (string category in subcategory)
                    {
                        if (prethodni != category)
                            sw.WriteLine(category);
                        prethodni = category;
                    }
                }*/

                for (int j = 0; j < getSubcategories.Length; j++)
                {
                    XmlDocument xmlDoc = eweDL.GetXml(getCategory, getSubcategories[j], getImages, getAttributes);
                    XmlNodeList nodeList = xmlDoc.DocumentElement.SelectNodes("product");
                    ErrorLog.LogMessage("preuzeti proizvodi");
                    foreach (XmlNode xmlNode in nodeList)
                    {
                        string code = xmlNode.SelectSingleNode("id").InnerText.Trim();
                        bool save = false;
                        for (int i = 0; i < codes.Length; i++)
                        {
                            if (codes[i] == code)
                            {
                                save = true;
                                break;
                            }
                        }
                        if (save)
                        {
                            Product product = new Product();
                            product.IsApproved = isApproved;
                            product.IsActive = isActive;
                            product.SupplierID = 1;
                            product.VatID = 4;
                            product.Categories = new List<Category>();
                            product.Categories.Add(category);
                            if (getAttributes)
                                product.Specification = specificationToHtml(xmlNode.SelectSingleNode("specifications").OuterXml);
                            else
                                product.Specification = string.Empty;
                            product.IsInStock = true;
                            bool isLocked = true;
                            isNew = false;

                            foreach (XmlNode xmlChildNode in xmlNode.ChildNodes)
                            {
                                switch (xmlChildNode.Name)
                                {
                                    case "id":
                                        {
                                            product.SupplierCode = xmlChildNode.InnerText.Trim();
                                            product.ProductID = productBL.GetProductIDBySupplierCode(product.SupplierCode);
                                            isLocked = productBL.IsLocked(product.ProductID);
                                            if (product.ProductID <= 0)
                                                isNew = true;
                                            break;
                                        }
                                    case "manufacturer":
                                        {
                                            if (!isLocked)
                                            {
                                                //if (product.ProductID <= 0)
                                                //{
                                                BrandBL brandBL = new BrandBL();
                                                Brand brand = brandBL.GetBrandByName(xmlChildNode.InnerText.Trim());
                                                if (brand == null)
                                                {
                                                    brand = new Brand();
                                                    brand.Name = xmlChildNode.InnerText.Trim();
                                                    brand.BrandID = brandBL.SaveBrand(brand);
                                                    //product.Brand.BrandID = brand.BrandID;
                                                }
                                                if (product.Brand == null)
                                                    product.Brand = new Brand();
                                                product.Brand.BrandID = brand.BrandID;
                                                //}
                                            }
                                            break;

                                        }
                                    case "name":
                                        {
                                            if (!isLocked)
                                                product.Name = xmlChildNode.InnerText.Trim();
                                            break;
                                        }

                                    case "category":
                                        {
                                            break;
                                        }

                                    /*case "subcategory":
                                        {
                                            CategoryBL categoryBL = new CategoryBL();
                                            //string categoryName = getSubcategory;  //(xmlChildNode.InnerText.Trim() == "NOTEBOOK") ? "LAPTOP" : xmlChildNode.InnerText.Trim();
                                            Category category = categoryBL.GetCategory(categoryName);
                                            product.Categories = new System.Collections.Generic.List<Category>();
                                            product.Categories.Add(category);

                                
                                            break;
                                        }*/

                                    case "price":
                                        {
                                            if (!isLocked)
                                            {
                                                product.Price = calculatePrice(double.Parse(xmlChildNode.InnerText.Replace('.', ',').Trim()), category.PricePercent);
                                                product.WebPrice = calculatePrice(double.Parse(xmlChildNode.InnerText.Replace('.', ',').Trim()), category.WebPricePercent);
                                            }
                                            break;
                                        }

                                    case "price_rebate":
                                        {
                                            break;
                                        }

                                    case "vat":
                                        {
                                            if (!isLocked)
                                            {
                                                switch (xmlChildNode.InnerText.Trim())
                                                {
                                                    case "20": { product.VatID = 4; break; }
                                                }
                                            }
                                            break;
                                        }

                                    case "ean":
                                        {
                                            if (!isLocked)
                                                product.Ean = xmlChildNode.InnerText.Trim();
                                            break;
                                        }

                                    case "images":
                                        {
                                            if (!isLocked)
                                            {
                                                //if (product.ProductID <= 0 || overwrite)
                                                //{
                                                foreach (XmlNode xmlImageNode in xmlChildNode.ChildNodes)
                                                {
                                                    if (product.Images == null)
                                                        product.Images = new System.Collections.Generic.List<string>();
                                                    if (xmlImageNode.Name == "image")
                                                    {
                                                        if (saveImageFromUrl(xmlImageNode.InnerText.Trim()))
                                                            product.Images.Add(Path.GetFileName(xmlImageNode.InnerText.Trim()));
                                                    }
                                                }
                                                //}
                                            }
                                            break;

                                        }

                                    case "specifications":
                                        {
                                            if (!isLocked)
                                            {
                                                if (product.ProductID <= 0 || overwrite)
                                                {
                                                    if (product.Categories != null)
                                                        if (product.Categories[0] != null)
                                                        {


                                                            //List<AttributeValue> attributeValues;
                                                            int attributeID;
                                                            foreach (XmlNode xmlSpecificationNode in xmlChildNode.ChildNodes)
                                                            {
                                                                if (xmlSpecificationNode.Name == "attribute_group")
                                                                {
                                                                    string attributeGroup = xmlSpecificationNode.Attributes[0].Value;
                                                                    foreach (XmlNode xmlAttributeNode in xmlSpecificationNode.ChildNodes)
                                                                    {
                                                                        if (xmlAttributeNode.Attributes[0].Value != "Programi / Ekskluzivne aplikacije / Servisi")
                                                                        {
                                                                            if ((attributeID = getAttributeID(attributes, attributeGroup + "-" + xmlAttributeNode.Attributes[0].Value)) == 0)
                                                                            {
                                                                                attributeID = addAttribute(product.Categories[0].CategoryID, attributeGroup + "-" + xmlAttributeNode.Attributes[0].Value);
                                                                                //attributes = attributeBL.GetAttributesForCategory(product.Categories[0].CategoryID);
                                                                                if (attributes == null)
                                                                                    attributes = new List<eshopBE.Attribute>();
                                                                                attributes.Add(new eshopBE.Attribute(attributeID, attributeGroup + "-" + xmlAttributeNode.Attributes[0].Value, false, false, 0));
                                                                            }

                                                                            if (product.Attributes == null)
                                                                                product.Attributes = new List<AttributeValue>();

                                                                            int attributeValueID = getAttributeValueID(attributeID, xmlAttributeNode.InnerText.Trim());
                                                                            product.Attributes.Add(new AttributeValue(attributeValueID, xmlAttributeNode.InnerText.Trim(), attributeID, 0, string.Empty, 0));
                                                                        }
                                                                    }
                                                                }
                                                            }
                                                        }
                                                }
                                            }
                                            break;
                                        }
                                }
                            }
                            product.Code = product.SupplierCode;
                            product.Description = string.Empty;

                            if (!isLocked)
                                if (productBL.SaveProduct(product) > 0)
                                    if (isNew)
                                        newProducts++;
                                    else
                                        updatedProducts++;
                        }
                    }
                }
            
                return "Dodato " + newProducts.ToString() + " proizvoda. Izmenjeno " + updatedProducts.ToString() + " proizvoda";
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex);
            }
            return string.Empty;
        }

        public DataTable parseProductsToDataTable(string category, string[] subcategories)
        {
            EweDL eweDL = new EweDL();
            DataTable products = new DataTable();
            products.Columns.Add("code");
            products.Columns.Add("ean");
            products.Columns.Add("manufacturer");
            products.Columns.Add("name");
            products.Columns.Add("price", typeof(double));
            products.Columns.Add("exists", typeof(bool));
            ProductDL productDL = new ProductDL();
            DataRow newRow;

            for (int i = 0; i < subcategories.Length; i++)
            {
                XmlDocument xmlDoc = eweDL.GetXml(category, subcategories[i], false, false);
                if (xmlDoc != null)
                {
                    XmlNodeList nodeList = xmlDoc.DocumentElement.SelectNodes("product");



                    foreach (XmlNode xmlNode in nodeList)
                    {
                        newRow = products.NewRow();
                        foreach (XmlNode xmlChildNode in xmlNode.ChildNodes)
                        {
                            switch (xmlChildNode.Name)
                            {
                                case "id":
                                    {
                                        newRow["code"] = xmlChildNode.InnerText.Trim();
                                        break;
                                    }

                                case "ean":
                                    {
                                        newRow["ean"] = xmlChildNode.InnerText.Trim();
                                        break;
                                    }

                                case "manufacturer":
                                    {
                                        newRow["manufacturer"] = xmlChildNode.InnerText.Trim();
                                        break;
                                    }

                                case "name":
                                    {
                                        newRow["name"] = xmlChildNode.InnerText.Trim();
                                        break;
                                    }

                                case "price":
                                    {
                                        newRow["price"] = xmlChildNode.InnerText.Trim().Replace('.', ',');
                                        break;
                                    }
                            }
                        }
                        newRow["exists"] = (productDL.GetProductIDBySupplierCode(newRow["code"].ToString()) > 0) ? true : false;
                        products.Rows.Add(newRow);
                    }
                }
            }
            return products;
        }

        public int ParseProductsForSaving(string category, string[] subcategories)
        {
            DataTable products = new DataTable();
            products.Columns.Add("code");
            products.Columns.Add("brand");
            products.Columns.Add("name");
            products.Columns.Add("price");
            products.Columns.Add("priceRebate");
            products.Columns.Add("vat");
            products.Columns.Add("category");
            products.Columns.Add("ean");
            products.Columns.Add("images");
            products.Columns.Add("specification");
            products.Columns.Add("subcategory");

            DataRow newRow;
            for(int i=0;i<subcategories.Length;i++)
            {
                XmlDocument xmlDoc = new EweDL().GetXml(category, subcategories[i], true, true);
                if(xmlDoc != null) { 
                XmlNodeList nodeList = xmlDoc.DocumentElement.SelectNodes("product");
                
                foreach(XmlNode xmlNode in nodeList)
                {
                    newRow = products.NewRow();
                    newRow["code"] = xmlNode.SelectSingleNode("id").InnerText.Trim();
                    newRow["brand"] = xmlNode.SelectSingleNode("manufacturer").InnerText.Trim();
                    newRow["name"] = xmlNode.SelectSingleNode("name").InnerText.Trim();
                    newRow["price"] = xmlNode.SelectSingleNode("price").InnerText.Replace('.',',').Trim();
                    newRow["priceRebate"] = xmlNode.SelectSingleNode("price_rebate").InnerText.Trim();
                    newRow["vat"] = xmlNode.SelectSingleNode("vat").InnerText.Trim();
                    newRow["category"] = xmlNode.SelectSingleNode("category").InnerText.Trim();
                    newRow["ean"] = xmlNode.SelectSingleNode("ean").InnerText.Trim();
                    newRow["images"] = xmlNode.SelectSingleNode("images").OuterXml.Trim();
                    newRow["specification"] = xmlNode.SelectSingleNode("specifications") != null ? xmlNode.SelectSingleNode("specifications").OuterXml.Trim() : string.Empty;
                    newRow["subcategory"] = xmlNode.SelectSingleNode("subcategory").InnerText.Trim();
                    if(xmlNode.SelectSingleNode("subcategory").InnerText.Trim() != string.Empty)
                        products.Rows.Add(newRow);
                }
                }
            }

            return new EweDL().SaveProducts(products, category);
            
        }

        public string parseProductsStockPrice(string eweCategory, string[] eweSubcategories, int categoryID, int supplierID)
        {
            EweDL eweDL = new EweDL();
            ProductDL productDL = new ProductDL();
            CategoryBL categoryBL = new CategoryBL();
            Category category = categoryBL.GetCategory(categoryID);
            productDL.SetInStock(supplierID, false, categoryID);
            int status = 0;


            for (int i = 0; i < eweSubcategories.Length; i++)
            {
                XmlDocument xmlDoc = eweDL.GetXml(eweCategory, eweSubcategories[i], false, false);
                XmlNodeList nodeList = xmlDoc.DocumentElement.SelectNodes("product");

                foreach (XmlNode xmlNode in nodeList)
                {
                    string supplierCode = xmlNode.SelectSingleNode("id").InnerText.Trim();
                    int productID;
                    if ((productID = productDL.GetProductIDBySupplierCode(supplierCode)) > 0)
                    {
                        if (!productDL.IsLocked(productID))
                        {
                            double price = calculatePrice(double.Parse(xmlNode.SelectSingleNode("price").InnerText.Replace('.', ',').Trim()), category.PricePercent);
                            double webPrice = calculatePrice(double.Parse(xmlNode.SelectSingleNode("price").InnerText.Replace('.', ',').Trim()), category.WebPricePercent);
                            status += productDL.UpdatePriceAndStock(productID, price, webPrice, true);
                        }
                    }
                }
            }
            return "Uspešno izmenjeno " + status.ToString() + " artikala.";
        }

        private bool saveImageFromUrl(string url)
        {
            bool exists = true;
            WebClient webClient = new WebClient();
            string filename = Path.GetFileName(url);
            string path = HttpContext.Current.Server.MapPath("~") + "images/";
            //if (!File.Exists(path + filename))
            //{
                ErrorLog.LogMessage(path + filename);
                webClient.DownloadFile(url, path + filename);
                
                if (File.Exists(path + filename))
                {
                    exists = true;
                    Image original = Image.FromFile(HttpContext.Current.Server.MapPath("~") + "/images/" + filename);
                    
                    //Image thumb = original.GetThumbnailImage(290, 232, null, IntPtr.Zero);
                    Image thumb = Common.CreateThumb(original, 290, 232);
                    thumb.Save(path + filename.Substring(0, filename.IndexOf(".jpg")) + "-main.jpg");

                    //thumb = original.GetThumbnailImage(110, 75, null, IntPtr.Zero);
                    thumb = Common.CreateThumb(original, 160, 110);
                    thumb.Save(path + filename.Substring(0, filename.IndexOf(".")) + "-list.jpg");

                    //thumb = original.GetThumbnailImage(30, 24, null, IntPtr.Zero);
                    thumb = Common.CreateThumb(original, 50, 40);
                    thumb.Save(path + filename.Substring(0, filename.IndexOf(".jpg")) + "-thumb.jpg");
                }
            //}

            return exists;
        }

        public DataTable GetEweCategories(int? parentCategoryID, int? categoryID)
        {
            EweDL eweDL = new EweDL();
            DataTable categories = eweDL.GetEweCategories(parentCategoryID, categoryID);
            if (parentCategoryID == null)
            {
                DataRow newRow = categories.NewRow();
                newRow["eweCategoryID"] = 0;
                newRow["name"] = "Odaberi kategoriju";
                newRow["parentID"] = 0;
                categories.Rows.InsertAt(newRow, 0);
            }
            return categories;
        }

        private string specificationToHtml(string xmlSpecification)
        {
            //xmlSpecification = @"<?xml version='1.0' encoding='UTF-8'?><specifications><attribute_group name='Ekran'><attribute name='Velicina ekrana'><value><![CDATA[ 11.6&quot; ]]></value></attribute><attribute name='Rezolucija'><value><![CDATA[ 1.366 x 768 ]]></value></attribute><attribute name='Format rezolucije'><value><![CDATA[ WXGA ]]></value></attribute><attribute name='Tip panela'><value><![CDATA[ LED, ]]></value><value><![CDATA[ HD ]]></value></attribute><attribute name='Ostalo'><value><![CDATA[ Acer ComfyView ]]></value></attribute></attribute_group><attribute_group name='Procesor / Cipset'><attribute name='Klasa procesora'><value><![CDATA[ AMDȮ E1 ]]></value></attribute><attribute name='Model'><value><![CDATA[ 2100 ]]></value></attribute><attribute name='Broj jezgara procesora'><value><![CDATA[ 2 ]]></value></attribute><attribute name='Broj niti'><value><![CDATA[ 2 ]]></value></attribute><attribute name='Radni takt procesora'><value><![CDATA[ 1.0 GHz ]]></value></attribute><attribute name='Ostalo'><value><![CDATA[ 28nm ]]></value></attribute></attribute_group><attribute_group name='RAM memorija'><attribute name='Memorija / RAM'><value><![CDATA[ 2GB ]]></value></attribute><attribute name='Tip'><value><![CDATA[ DDR3 ]]></value></attribute><attribute name='Broj slotova'><value><![CDATA[ 1 ]]></value></attribute><attribute name='Popunjeni slotovi'><value><![CDATA[ 1 ]]></value></attribute><attribute name='Maksimalno podr�ano'><value><![CDATA[ 4GB ]]></value></attribute><attribute name='Napomena'><value><![CDATA[ Low-Voltage memorija ]]></value></attribute></attribute_group><attribute_group name='Grafika'><attribute name='Grafika'><value><![CDATA[ AMD� Radeon� ]]></value></attribute><attribute name='Model grafike'><value><![CDATA[ Radeon� HD 8210 ]]></value></attribute><attribute name='Koliina grafi�ke memorije'><value><![CDATA[ 512MB ]]></value></attribute><attribute name='Tip grafi�ke memorije'><value><![CDATA[ Deljena ]]></value></attribute></attribute_group><attribute_group name='Hard disk / SSD / Optika'><attribute name='Kapacitet diska'><value><![CDATA[ 500GB ]]></value></attribute><attribute name='Interfejs'><value><![CDATA[ SATA ]]></value></attribute><attribute name='Brzina obrtaja diska'><value><![CDATA[ 5.400rpm ]]></value></attribute><attribute name='Opti�ki ure�aj'><value><![CDATA[ Bez opti�kog ure�aja ]]></value></attribute></attribute_group><attribute_group name='Mrea'><attribute name='Wi-Fi'><value><![CDATA[ Da ]]></value></attribute><attribute name='Bei�ni mreni standardi'><value><![CDATA[ IEEE 802.11b, ]]></value><value><![CDATA[ IEEE 802.11g, ]]></value><value><![CDATA[ IEEE 802.11n ]]></value></attribute><attribute name='Model'><value><![CDATA[ Acer InviLink螙 Nplify� ]]></value></attribute><attribute name='Bluetooth�'><value><![CDATA[ 2.1 ]]></value></attribute><attribute name='�ina mrea (LAN)'><value><![CDATA[ 10/100Mbps (Fast ethernet) ]]></value></attribute></attribute_group><attribute_group name='Priklju�ci / Slotovi'><attribute name='HDMI priklju�ci'><value><![CDATA[ 1x HDMI ]]></value></attribute><attribute name='VGA D-sub'><value><![CDATA[ 1 ]]></value></attribute><attribute name='Ukupno USB priklju�aka'><value><![CDATA[ 3 ]]></value></attribute><attribute name='USB 3.0 priklju�ci'><value><![CDATA[ 1 ]]></value></attribute><attribute name='USB 2.0 priklju�ci'><value><![CDATA[ 2 ]]></value></attribute><attribute name='RJ-45 (LAN)'><value><![CDATA[ 1 ]]></value></attribute><attribute name='Audio'><value><![CDATA[ 1x 3.5mm (izlaz i mikrofon) ]]></value></attribute><attribute name='�ita� kartica'><value><![CDATA[ Da ]]></value></attribute><attribute name='Ostali priklju�ci / Slotovi'><value><![CDATA[ Kensington security ]]></value></attribute></attribute_group><attribute_group name='Kamera'><attribute name='Rezolucija'><value><![CDATA[ 1 Megapiksel ]]></value></attribute><attribute name='Snimanje videa'><value><![CDATA[ HD 720p ]]></value></attribute><attribute name='Model'><value><![CDATA[ Acer Crystal Eye HD ]]></value></attribute></attribute_group><attribute_group name='Audio'><attribute name='Zvu�nici'><value><![CDATA[ 2.0 ]]></value></attribute><attribute name='Mikrofon'><value><![CDATA[ Ugra�en ]]></value></attribute><attribute name='Ostalo'><value><![CDATA[ High Definition ]]></value></attribute></attribute_group><attribute_group name='Tastatura'><attribute name='Slovni raspored tastature'><value><![CDATA[ EN (US) ]]></value></attribute><attribute name='Ostalo'><value><![CDATA[ Multi-touch touchpad, ]]></value><value><![CDATA[ Acer FineTip ]]></value></attribute></attribute_group><attribute_group name='Baterija'><attribute name='Tip'><value><![CDATA[ Litijum-jonska ]]></value></attribute><attribute name='Broj �elija baterije'><value><![CDATA[ 4 ]]></value></attribute><attribute name='Kapacitet'><value><![CDATA[ 2.500mAh ]]></value></attribute></attribute_group><attribute_group name='Softver'><attribute name='Operativni sistem'><value><![CDATA[ Linux ]]></value></attribute><attribute name='Edicija'><value><![CDATA[ Linpus ]]></value></attribute></attribute_group><attribute_group name='Fizi�ke karakteristike'><attribute name='Masa'><value><![CDATA[ 1.2kg ]]></value></attribute><attribute name='Boja'><value><![CDATA[ Crna ]]></value></attribute><attribute name='Opis boje'><value><![CDATA[ Glossy Black ]]></value></attribute></attribute_group><attribute_group name='Garancija'><attribute name='Garancija'><value><![CDATA[ 2 godine ]]></value></attribute></attribute_group></specifications>";
            //string xslt = "@<?xml version='1.0' encoding='utf-8'?><xsl:stylesheet version='1.0' xmlns:xsl='http://www.w3.org/1999/XSL/Transform' xmlns:msxsl='urn:schemas-microsoft-com:xslt' exclude-result-prefixes='msxsl'><xsl:output method='xml' indent='yes'/><xsl:template match='specifications'><table><xsl:for-each select='attribute_group'><tr><td><xsl:value-of select='@name'/></td></tr><xsl:for-each select='current()/attribute'><tr><td><xsl:value-of select='@name'/><xsl:value-of select='value'/></td></tr></xsl:for-each></xsl:for-each></table></xsl:template></xsl:stylesheet>";
            XslCompiledTransform transform = new XslCompiledTransform();
            //using (XmlReader reader = XmlReader.Create(new StringReader(xslt)))
            //{
            transform.Load(HttpContext.Current.Server.MapPath("~") + "eweXslt.xslt");
            //}

            StringWriter html = new StringWriter();
            if(xmlSpecification != string.Empty)
            { 
                using (XmlReader reader = XmlReader.Create(new StringReader(xmlSpecification)))
                {
                    transform.Transform(reader, null, html);
                }
            }
            return html.ToString();
        }

        private double calculatePrice(double supplierPrice, double percent)
        {
            return double.Parse(((int)(supplierPrice * (percent / 100 + 1) * 1.2) / 100 * 100 - 10).ToString());
        }

        public void UpdateEweCategories()
        {
            EweDL eweDL = new EweDL();
            XmlDocument xmlDoc = eweDL.GetXml(string.Empty, string.Empty, false, false);

            DeleteEweCategories();
            XmlNodeList nodeList = xmlDoc.DocumentElement.SelectNodes("product");
            foreach (XmlNode xmlNode in nodeList)
            {
                eweDL.SaveCategory(xmlNode.SelectSingleNode("category").InnerText.Trim(), null);
                eweDL.SaveCategory(xmlNode.SelectSingleNode("subcategory").InnerText.Trim(), xmlNode.SelectSingleNode("category").InnerText.Trim());
            }
        }

        public int SaveSelected(string categoryIDs, string selected)
        {
            EweDL eweDL = new EweDL();
            return eweDL.SaveSelected(categoryIDs.Split(','), selected.Split(','));
        }

        public DataTable GetNewCategories()
        {
            EweDL eweDL = new EweDL();
            return eweDL.GetNewCategories();
        }

        public int GetEweCategoryForCategory(int categoryID)
        {
            EweDL eweDL = new EweDL();
            return eweDL.GetEweCategoryForCategory(categoryID);
        }

        public int SaveEweCategoryForCategory(int categoryID, int eweCategoryID, bool isCategory)
        {
            EweDL eweDL = new EweDL();
            return eweDL.SaveEweCategoryForCategory(categoryID, eweCategoryID, isCategory);
        }

        public int DeleteEweCategories()
        {
            EweDL eweDL = new EweDL();
            return eweDL.DeleteEweCategories();
        }

        public int DeleteCategoryEweCategory(int categoryID)
        {
            EweDL eweDL = new EweDL();
            return eweDL.DeleteCategoryEweCategory(categoryID);
        }

        public DataTable GetProducts(string category, string[] subcategories)
        {
            return new EweDL().GetProducts(category, subcategories);
        }

        public bool SaveProduct(string supplierCode, bool isApproved, bool isActive, int categoryID)
        {
            DataTable eweProduct = new EweDL().GetProductBySupplierCode(supplierCode);

            Category category = new CategoryBL().GetCategory(categoryID);
            List<eshopBE.Attribute> attributes = new AttributeBL().GetAttributesForCategory(categoryID);

            Product product = new Product();
            product.IsApproved = isApproved;
            product.IsActive = isActive;
            product.SupplierID = 1;
            product.SupplierCode = supplierCode;
            product.VatID = 4;
            product.Categories = new List<Category>();
            product.Categories.Add(category);
            product.Specification = specificationToHtml(eweProduct.Rows[0]["specification"].ToString());
            product.IsInStock = true;
            bool isNew = false;
            bool isLocked = false;
            product.Code = product.SupplierCode;
            product.Description = string.Empty;

            product.ProductID = new ProductBL().GetProductIDBySupplierCode(product.SupplierCode);
            if (product.ProductID <= 0)
                isNew = true;
            isLocked = new ProductBL().IsLocked(product.ProductID);

            Brand brand = new BrandBL().GetBrandByName(eweProduct.Rows[0]["brand"].ToString());
            if(brand == null)
            {
                brand = new Brand();
                brand.Name = eweProduct.Rows[0]["brand"].ToString();
                brand.BrandID = new BrandBL().SaveBrand(brand);
            }
            if (product.Brand == null)
                product.Brand = new Brand();
            product.Brand.BrandID = brand.BrandID;

            product.Name = eweProduct.Rows[0]["name"].ToString();
            product.Price = calculatePrice(double.Parse(eweProduct.Rows[0]["price"].ToString()), category.PricePercent);
            product.WebPrice = calculatePrice(double.Parse(eweProduct.Rows[0]["price"].ToString()), category.WebPricePercent);
            product.Ean = eweProduct.Rows[0]["ean"].ToString();
            product.SupplierPrice = double.Parse(eweProduct.Rows[0]["price"].ToString());

            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(eweProduct.Rows[0]["images"].ToString());
            XmlNode xmlImagesNode = xmlDoc.SelectSingleNode("images");
            foreach (XmlNode xmlImageNode in xmlImagesNode.ChildNodes)
            {
                if (product.Images == null)
                    product.Images = new List<string>();
                if(xmlImageNode.Name == "image")
                {
                    if (saveImageFromUrl(xmlImageNode.InnerText.Trim()))
                        product.Images.Add(Path.GetFileName(xmlImageNode.InnerText.Trim()));
                }
            }

            int attributeID;
            if(eweProduct.Rows[0]["specification"].ToString() != string.Empty) { 
                xmlDoc.LoadXml(eweProduct.Rows[0]["specification"].ToString());
                XmlNode xmlSpecificationsNode = xmlDoc.SelectSingleNode("specifications");
                foreach(XmlNode xmlSpecificationNode in xmlSpecificationsNode.ChildNodes)
                {
                    if(xmlSpecificationNode.Name == "attribute_group")
                    {
                        string attributeGroup = xmlSpecificationNode.Attributes[0].Value;
                        foreach(XmlNode xmlAttributeNode in xmlSpecificationNode.ChildNodes)
                        {
                            if(xmlAttributeNode.Attributes[0].Value != "Programi / Ekskluzivne aplikacije / Servisi")
                            {
                                if((attributeID = getAttributeID(attributes, attributeGroup + "-" + xmlAttributeNode.Attributes[0].Value)) == 0)
                                {
                                    attributeID = addAttribute(product.Categories[0].CategoryID, attributeGroup + "-" + xmlAttributeNode.Attributes[0].Value);
                                    if (attributes == null)
                                        attributes = new List<eshopBE.Attribute>();
                                    attributes.Add(new eshopBE.Attribute(attributeID, attributeGroup + "-" + xmlAttributeNode.Attributes[0].Value, false, false, 0));
                                }
                                if (product.Attributes == null)
                                    product.Attributes = new List<AttributeValue>();

                                int attributeValueID = getAttributeValueID(attributeID, xmlAttributeNode.InnerText.Trim());
                                product.Attributes.Add(new AttributeValue(attributeValueID, xmlAttributeNode.InnerText.Trim(), attributeID, 0, string.Empty, 0));
                            }
                        }
                    }
                }
            }

            if (!isLocked)
                if (new ProductBL().SaveProduct(product) > 0)
                    return true;
            return false;
        }
    }
}
