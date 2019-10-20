using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.ComponentModel;
using System.Net.Mail;
using System.Net;
using System.Web.Configuration;
using System.Configuration;
using System.Collections.Specialized;
using System.Xml;
using System.Web;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using eshopBE;
using AegisImplicitMail;

namespace eshopUtilities
{
    public static class Common
    {
        public static DataTable ConvertToDataTable<T>(this IEnumerable<T> data)
        {
            List<IDataRecord> list = data.Cast<IDataRecord>().ToList();

            PropertyDescriptorCollection props = null;
            DataTable table = new DataTable();
            if (list != null && list.Count > 0)
            {
                props = TypeDescriptor.GetProperties(list[0]);
                for (int i = 0; i < props.Count; i++)
                {
                    PropertyDescriptor prop = props[i];
                    table.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
                }
            }
            if (props != null)
            {
                object[] values = new object[props.Count];
                foreach (T item in data)
                {
                    for (int i = 0; i < values.Length; i++)
                    {
                        values[i] = props[i].GetValue(item) ?? DBNull.Value;
                    }
                    table.Rows.Add(values);
                }
            }
            return table;
        }

        public static int SendOrderConfirmationMail(string email, string name, Order order, Settings settings)
        {
            try
            {
                MailMessage message = new MailMessage();
                //MimeMailMessage message = new MimeMailMessage();
                //message.From = new MailAddress("office@pinservis.com");
                ////message.From = new MailAddress(ConfigurationManager.AppSettings["orderEmail"], ConfigurationManager.AppSettings["companyName"]);
                message.From = new MailAddress(ConfigurationManager.AppSettings["orderEmail"]);
                message.To.Add(new MailAddress(email));
                //message.CC.Add(new MailAddress("info@pinservis.co.rs.gladiolus.arvixe.com"));
                message.Subject = "Potvrda narudžbine - " + ConfigurationManager.AppSettings["companyName"];
                message.BodyEncoding=Encoding.UTF8;
                message.IsBodyHtml = true;
                StringBuilder body = new StringBuilder();
                body.Append("<div style='font-family:verdana;font-size:0.9em'>");
                body.Append("<img src='" + ConfigurationManager.AppSettings["logoUrl"].ToString() + "' style='width:50px;margin-bottom:20px' />");
                body.Append("<h1 style='font-size:1em;width:100%;text-align:center'>Potvrda porudžbine</h1>");
                body.Append("<br/><strong>Poštovani " + name + "</strong><br><br>Vaša porudžbina je uspešno prosleđena.");
                body.Append("<br/><br/>U nastavku možete videti detalje Vaše porudžbine.");
                body.Append("<br/><br/>");
                body.Append("<table width='100%' border='0' cellspacing='0' style='font-family:verdana;font-size:0.9em'>");
                body.Append("<tr bgcolor='#cccccc'>");
                body.Append("<th align='center' style='width:50px'>Rbr</th><th>Naziv</th><th style='width:100px'>Količina</th><th style='width:100px'>Cena</th><th style='width:100px'>Ukupno</th>");
                body.Append("</tr>");
                double ukupno=0;
            
                for(int i=0;i<order.Items.Count;i++)
                {
                    ukupno += order.Items[i].UserPrice * order.Items[i].Quantity;
                    body.Append("<tr height='20px' valign='middle'>");
                    body.Append("<td align='center'>" + (i + 1).ToString() + "</td><td>" + "<a href='" + ConfigurationManager.AppSettings["webShopUrl"] + "'" + order.Items[i].Product.Url + "' style='color:#d3232e'>" + order.Items[i].Product.Name.ToString() + ", " + order.Items[i].Product.Description + ", " + order.Items[i].Product.Code + "</a>" + "</td><td align='right'>" + order.Items[i].Quantity.ToString() + "</td><td align='right'>" + string.Format("{0:N2}", order.Items[i].UserPrice) + "</td><td align='right'>" + string.Format("{0:N2}", order.Items[i].Quantity * order.Items[i].UserPrice) + "</td>");
                    body.Append("</tr>");
                }
                body.Append("</table>");
                body.Append("<div style='text-align:right;'>");
                body.Append("<br/>");

                body.Append("Ukupno: " + string.Format("{0:N2}", ukupno));
                body.Append("<br />");
                //double userDiscountValue = discountTypeID == 1 ? ukupno * userDiscount / 100 : userDiscount;
                body.Append("Popust: " + string.Format("{0:N2}", order.UserDiscountValue));
                body.Append("<br />");
                body.Append("Ukupno: " + string.Format("{0:N2}", ukupno - order.UserDiscountValue));
                body.Append("<br />");
                //body.Append("Dostava: " + 
                    //(bool.Parse(ConfigurationManager.AppSettings["calculateDelivery"]) 
                        //? string.Format("{0:N2}", ukupno - order.UserDiscountValue > double.Parse(ConfigurationManager.AppSettings["freeDeliveryTotalValue"])
                            //? 0 : double.Parse(ConfigurationManager.AppSettings["deliveryCost"]))
                                //: ukupno - order.UserDiscountValue > double.Parse(ConfigurationManager.AppSettings["freeDeliveryTotalValue"]) 
                                        //? "0,00" : " Po cenovniku kurirske službe"));

                body.Append("Dostava: " +
                    (bool.Parse(ConfigurationManager.AppSettings["calculateDelivery"])
                        ? string.Format("{0:N2}", ukupno - order.UserDiscountValue > settings.FreeDeliveryTotalValue
                            ? 0 : settings.DeliveryCost)
                        : ukupno - order.UserDiscountValue > settings.FreeDeliveryTotalValue
                            ? "0,00" : " Po cenovniku kurirske službe"));
                body.Append("<br />");

                //body.Append("Ukupno sa dostavom: " + 
                    //(bool.Parse(ConfigurationManager.AppSettings["calculateDelivery"]) 
                        //? string.Format("{0:N2}", ukupno - order.UserDiscountValue + 
                            //(ukupno > double.Parse(ConfigurationManager.AppSettings["freeDeliveryTotalValue"]) 
                                //? 0 : double.Parse(ConfigurationManager.AppSettings["deliveryCost"]))) 
                        //: (string.Format("{0:N2}", ukupno - order.UserDiscountValue)) + (ukupno - order.UserDiscountValue > double.Parse(ConfigurationManager.AppSettings["freeDeliveryTotalValue"]) 
                            //? "" : " + cena dostave")));

                body.Append("Ukupno sa dostavom: " +
                    (bool.Parse(ConfigurationManager.AppSettings["calculateDelivery"])
                        ? string.Format("{0:N2}", ukupno - order.UserDiscountValue +
                            (ukupno > settings.FreeDeliveryTotalValue
                                ? 0 : settings.DeliveryCost))
                        : (string.Format("{0:N2}", ukupno - order.UserDiscountValue)) + (ukupno - order.UserDiscountValue > settings.FreeDeliveryTotalValue
                            ? "" : " + cena dostave")));
                body.Append("</div>");
                body.Append("<br/>");
                if(order.UserDiscountValue > 0)
                    body.Append("<p><strong>Odobren vam je popust u iznosu od: " + string.Format("{0:N2}", order.UserDiscountValue) + " dinara</strong></p>");
                if(order.Lastname != string.Empty || order.Firstname != string.Empty)
                    body.Append("<p><strong>Prezime i ime: </strong>" + order.Lastname + " " + order.Firstname + "</p>");
                else if (order.Name != string.Empty)
                {
                    body.Append("<p><strong>Naziv: </strong>" + order.Name + "</p>");
                    body.Append("<p><strong>PIB: </strong>" + order.Pib + "</p>");
                }
                body.Append("<p><strong>Adresa: </strong>" + order.Address + " " + order.Code + " " + order.City + " " + order.Zip + "</p>");
                body.Append("<p><strong>Telefon: </strong>" + order.Phone + "</p>");
                body.Append("<p><strong>Način plaćanja: </strong>" + order.Payment.Name + "</p>");
                body.Append("<p><strong>Način preuzimanja: </strong>" + order.Delivery.Name + "</p>");
                body.Append("<p><strong>Napomena: </strong>" + order.Comment + "</p>");
            
                body.Append("<br/><br/>Vaša online prodavnica ");
                body.Append("<span style='font-weight:bold;color:#174e87'>" + ConfigurationManager.AppSettings["companyName"] + "</span>");
                body.Append("</div>");
                message.Body = body.ToString();

                AlternateView plainView = AlternateView.CreateAlternateViewFromString("Vaša porudžbina je uspešno prosleđena.", null, "text/plain");
                AlternateView htmlView = AlternateView.CreateAlternateViewFromString(message.Body, null, "text/html");

                message.AlternateViews.Add(plainView);
                message.AlternateViews.Add(htmlView);

                message.Body = string.Empty;
                message.Headers.Add("Message-Id", "<" + Guid.NewGuid().ToString() + "@" + ConfigurationManager.AppSettings["webShopDomain"] + ">");


                SmtpClient smtp = getSmtp(ConfigurationManager.AppSettings["orderEmail"], "orderEmail");
                //MimeMailer smtp = getAegisSmtp(ConfigurationManager.AppSettings["orderEmail"], "orderEmail");
                smtp.Send(message);

                return 0;
            }
            catch(Exception ex)
            {
                throw new BLException("Nije moguće poslati mail", ex);
            }
        }

        public static int SendUserCreatedConfirmationMail(string email, string password)
        {
            try
            {
                MailMessage message = new MailMessage();
                //MimeMailMessage message = new MimeMailMessage();
                //message.From = new MailAddress("office@pinservis.com");
                //message.From = new MailAddress(ConfigurationManager.AppSettings["infoEmail"].ToString(), ConfigurationManager.AppSettings["companyName"].ToString());
                message.From = new MailAddress(ConfigurationManager.AppSettings["infoEmail"]);
                //message.From = new MailAddress("office@pinshop.co.rs");
                message.To.Add(new MailAddress(email));
                message.Subject = "Korisnički nalog kreiran - " + ConfigurationManager.AppSettings["companyName"];
                message.BodyEncoding = Encoding.UTF8;
                StringBuilder body = new StringBuilder();
                body.Append("<img src='" + ConfigurationManager.AppSettings["logoUrl"].ToString() + "' style='width:150px;margin-bottom:20px' /><br/>Poštovani,<br/><br/>Vaš korisnički nalog na web portalu <a href='" + ConfigurationManager.AppSettings["webShopUrl"] + "'>" + ConfigurationManager.AppSettings["companyName"] + "</a> je uspešno kreiran.<br/><br/>");
                body.Append("<br/>Za pristup portalu možete koristiti sledeće korisničke podatke:<br/><br/>Vaše korisničko ime je: <b>" + email + "</b><br>Vaša šifra je: <b>" + password + "</b><br/>");
                body.Append("<br/>Za prijavu koristite stranicu <a href='" + ConfigurationManager.AppSettings["webShopLoginUrl"] + "'>Prijava</a>");
                body.Append("<br/><br/>Vaša online prodavnica ");
                body.Append("<span style='font-weight:bold;color:#d3232e'>" + ConfigurationManager.AppSettings["companyName"] + "</span>");
                message.Body = body.ToString();
                message.IsBodyHtml = true;
                AlternateView plainView = AlternateView.CreateAlternateViewFromString("Poštovani, Vaš korisnički nalog je uspešno kreiran.\r\nZa pristup portalu možete korisniti sledeće korisničke podatke:\r\n" +
                    "Korisničko ime: " + email + "\r\n" + "Korisnička šifra: " + password, null, "text/plain");
                AlternateView htmlView = AlternateView.CreateAlternateViewFromString(message.Body, null, "text/html");
                message.AlternateViews.Add(plainView);
                message.AlternateViews.Add(htmlView);
                message.Body = string.Empty;
                message.Headers.Add("Message-Id", "<" + Guid.NewGuid().ToString() + "@" + ConfigurationManager.AppSettings["webShopDomain"] + ">");

                SmtpClient smtp = getSmtp(message.From.Address.ToString(), "infoEmail");
                //MimeMailer smtp = getAegisSmtp(message.From.Address.ToString(), "infoEmail");
                smtp.Send(message);
            }
            catch (Exception ex)
            {
                throw new BLException("Nije moguće poslati mail", ex);
            }
            return 0;
        }

        public static bool SendPasswordResetMail(string username, string token)
        {
            try
            {
                MailMessage message = new MailMessage();
                //MimeMailMessage message = new MimeMailMessage();
                //message.From = new MailAddress(ConfigurationManager.AppSettings["infoEmail"].ToString(), ConfigurationManager.AppSettings["companyName"].ToString());
                message.From = new MailAddress(ConfigurationManager.AppSettings["infoEmail"]);
                message.To.Add(new MailAddress(username));
                message.Subject = "Resetovanje korisničke šifre";
                message.BodyEncoding = Encoding.UTF8;
                StringBuilder body = new StringBuilder();
                body.Append("Kliknite na link da bi kreirali novu korisničku šifru: <a href='" + ConfigurationManager.AppSettings["webShopUrl"].ToString() + "/kreiranje-korisnicke-sifre?id=" + token + "'>Kreiranje korisničke šifre</a>");
                message.IsBodyHtml = true;
                message.Body = body.ToString();


                SmtpClient smtp = getSmtp(message.From.Address.ToString(), "infoEmail");
                //MimeMailer smtp = getAegisSmtp(message.From.Address.ToString(), "infoEmail");
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                smtp.Send(message);
                return true;
            }
            catch(Exception ex)
            {
                throw new BLException("Nije moguće poslati mail", ex);
                return false;
            }
        }

        public static MimeMailer getAegisSmtp(string email, string type)
        {
            ////SmtpClient smtp = new SmtpClient();
            MimeMailer smtp = new MimeMailer(ConfigurationManager.AppSettings["smtp"], int.Parse(ConfigurationManager.AppSettings["smtpPort"]));
            NetworkCredential networkCredential = new NetworkCredential(email, ConfigurationManager.AppSettings[type + "Password"].ToString());
            //NetworkCredential networkCredential = new NetworkCredential("office@pinshop.co.rs", "webprodaja023");
            ////smtp.UseDefaultCredentials = false;
            ////smtp.Credentials = networkCredential;
            ////smtp.Host = ConfigurationManager.AppSettings["smtp"].ToString();
            //smtp.Host = "mail.pinshop.co.rs";
            ////smtp.Port = int.Parse(ConfigurationManager.AppSettings["smtpPort"]);
            ////smtp.EnableSsl = bool.Parse(ConfigurationManager.AppSettings["smtpSsl"]);
            //smtp.SslType = bool.Parse(ConfigurationManager.AppSettings["smtpSsl"]) ? SslMode.Tls : SslMode.None;
            smtp.SslType = SslMode.None;
            //smtp.AuthenticationMode = bool.Parse(ConfigurationManager.AppSettings["smtpSsl"]) ? AuthenticationType.Base64 : AuthenticationType.PlainText;
            smtp.AuthenticationMode = AuthenticationType.Base64;
            smtp.User = email;
            smtp.Password = ConfigurationManager.AppSettings[type + "Password"];
            //smtp.EnableImplicitSsl = true;
            //SslMode sslMode = smtp.DetectSslMode();
            //SslMode sslMode = DetectSslType();
            

            return smtp;
        }

        private static SmtpClient getSmtp(string email, string type)
        {
            SmtpClient smtp = new SmtpClient();
            NetworkCredential networkCredentials = new NetworkCredential(email, ConfigurationManager.AppSettings[type + "Password"]);
            smtp.UseDefaultCredentials = false;
            smtp.Credentials = networkCredentials;
            smtp.Host = ConfigurationManager.AppSettings["smtp"];
            smtp.Port = int.Parse(ConfigurationManager.AppSettings["smtpPort"]);
            smtp.EnableSsl = bool.Parse(ConfigurationManager.AppSettings["smtpSsl"]);

            return smtp;
        }

        public static string CreateFriendlyUrl(string url)
        {
            url = url.ToLower();
            char[] notAllwed = { 'š', 'ć', 'č', 'ž', ',', '.', '"', ' ', '(', ')', '*' };
            char[] replacement = { 's', 'c', 'c', 'z', '-', '-', '-', '-', '-', '-', '-' };

            url = url.Replace("\n", "-");
            for (int i = 0; i < notAllwed.Length; i++)
                url = url.Replace(notAllwed[i], replacement[i]);

            url = url.Replace("đ", "dj");

            url = url.Replace("--", "-");
            url = url.Replace("---", "-");
            url = url.Trim();

            return url;
        }

        public static void SendOrder()
        {
            try
            {
                MailMessage mail = new MailMessage();
                //MimeMailMessage mail = new MimeMailMessage();
                mail.From = new MailAddress(ConfigurationManager.AppSettings["infoEmail"]);
                mail.To.Add(new MailAddress(ConfigurationManager.AppSettings["infoEmail"]));
                mail.Subject = "Nova porudžbina";
                mail.BodyEncoding=Encoding.UTF8;
                mail.Body="Imate novu porudžbinu sa sajta.<br/>Sve porudžbine možete videti na stranici <a href='" + ConfigurationManager.AppSettings["webShopUrl"] + "/administrator/orders.aspx'>" + ConfigurationManager.AppSettings["webShopUrl"] + "/administrator/orders.aspx</a>";
                mail.IsBodyHtml=true;

                SmtpClient smtp = getSmtp(ConfigurationManager.AppSettings["infoEmail"].ToString(), "infoEmail");
                //MimeMailer smtp = getAegisSmtp(ConfigurationManager.AppSettings["infoEmail"].ToString(), "infoEmail");
                smtp.Send(mail);
            }
            catch(Exception ex)
            {
                throw new BLException("Nije moguće poslati mail", ex);
            }
        }

        public static void AddUrlRewrite(string url, string page)
        {
            //Configuration configuration = WebConfigurationManager.OpenWebConfiguration("~");
            //NameValueCollection collection = WebConfigurationManager.GetSection("rewriter") as NameValueCollection;
            //System.Configuration.DefaultSection rewriter = (System.Configuration.DefaultSection)configuration.GetSection("rewriter");
            //foreach (string key in rewriter.ElementInformation.Properties.Keys)
                //continue;

            XmlDocument doc = new XmlDocument();
            doc.Load(HttpContext.Current.Server.MapPath("~/Web.config"));

            XmlNode rewriterNode = doc.DocumentElement.SelectSingleNode("rewriter");

            XmlElement element = doc.CreateElement("rewrite");
            element.SetAttribute("url", "^/" + url + "/?$");
            element.SetAttribute("to", "~/" + page + "?url=$0");
            element.SetAttribute("processing", "stop");

            rewriterNode.AppendChild(element);

            doc.Save(HttpContext.Current.Server.MapPath("~/Web.config"));
        }

        public static void RemoveUrlRewrite(string url)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(HttpContext.Current.Server.MapPath("~/Web.config"));

            XmlNode rewriteNode = doc.DocumentElement.SelectSingleNode("rewriter");
            foreach (XmlNode node in rewriteNode.ChildNodes)
                if (node.Attributes["url"].Value.ToString().Contains(url))
                {
                    rewriteNode.RemoveChild(node);
                    break;
                }
            doc.Save(HttpContext.Current.Server.MapPath("~/Web.config"));
        }

        public static Image CreateThumb(Image imgPhoto, int Width, int Height)
        {
            int sourceWidth = imgPhoto.Width;
            int sourceHeight = imgPhoto.Height;
            int sourceX = 0;
            int sourceY = 0;
            int destX = 0;
            int destY = 0;

            float nPercent = 0;
            float nPercentW = 0;
            float nPercentH = 0;

            nPercentW = ((float)Width / (float)sourceWidth);
            nPercentH = ((float)Height / (float)sourceHeight);
            if (nPercentH < nPercentW)
            {
                nPercent = nPercentH;
                destX = System.Convert.ToInt16((Width -
                              (sourceWidth * nPercent)) / 2);
            }
            else
            {
                nPercent = nPercentW;
                destY = System.Convert.ToInt16((Height -
                              (sourceHeight * nPercent)) / 2);
            }

            int destWidth = (int)(sourceWidth * nPercent);
            int destHeight = (int)(sourceHeight * nPercent);

            Bitmap bmPhoto = new Bitmap(Width, Height,
                              PixelFormat.Format24bppRgb);
            bmPhoto.SetResolution(imgPhoto.HorizontalResolution,
                             imgPhoto.VerticalResolution);

            Graphics grPhoto = Graphics.FromImage(bmPhoto);
            grPhoto.Clear(Color.White);
            grPhoto.InterpolationMode =
                    InterpolationMode.HighQualityBicubic;

            grPhoto.DrawImage(imgPhoto,
                new Rectangle(destX, destY, destWidth, destHeight),
                new Rectangle(sourceX, sourceY, sourceWidth, sourceHeight),
                GraphicsUnit.Pixel);

            grPhoto.Dispose();
            return bmPhoto;
        }

        public static DateTime ConvertToLocalTime(DateTime UTCDateTime)
        {
            //TimeZoneInfo tzi = TimeZoneInfo.FindSystemTimeZoneById("Central European Standard Time");
            return TimeZoneInfo.ConvertTimeFromUtc(UTCDateTime, TimeZoneInfo.FindSystemTimeZoneById("Central European Standard Time"));
        }

        public static DataTable ConvertToLocalTime(ref DataTable table)
        {
            if (table != null)
            {
                for (int i = 0; i < table.Rows.Count; i++)
                    for (int j = 0; j < table.Columns.Count; j++)
                        if (table.Rows[i][j].GetType() == typeof(System.DateTime))
                            table.Rows[i][j] = ConvertToLocalTime(DateTime.Parse(table.Rows[i][j].ToString()));
            }
            return table;

        }

        public static bool IsValidEmail(string email)
        {
            return true;
        }

        public static IEnumerable<DateTime> EachDay(DateTime dateFrom, DateTime dateTo)
        {
            for (var day = dateFrom.Date; day.Date <= dateTo.Date; day = day.AddDays(1))
                yield return day;
        }

        public static void SendNewOrderNotification(string orderID, Order order, Settings settings)
        {
            try
            {
                MailMessage mail = new MailMessage();
                //MimeMailMessage mail = new MimeMailMessage();
                //mail.From = new MailAddress(ConfigurationManager.AppSettings["infoEmail"].ToString(), ConfigurationManager.AppSettings["companyName"].ToString());
                mail.From = new MailAddress(ConfigurationManager.AppSettings["infoEmail"]);
                if (ConfigurationManager.AppSettings["webShopDestinationEmail"] == null || ConfigurationManager.AppSettings["webShopDestinationEmail"] == string.Empty)
                    mail.To.Add(new MailAddress(ConfigurationManager.AppSettings["infoEmail"]));
                else
                {
                    foreach (string destinationEmail in ConfigurationManager.AppSettings["webShopDestinationEmail"].Split(','))
                        mail.To.Add(new MailAddress(destinationEmail));
                }
                //mail.To.Add(new MailAddress(ConfigurationManager.AppSettings["infoEmail"]));
                //foreach (string emailAddress in ConfigurationManager.AppSettings["infoEmail"].Split(';'))
                    //mail.To.Add(new MailAddress(emailAddress));
                //mail.Subject = "Nova porudžbina";
                StringBuilder body = new StringBuilder();
                body.Append("<strong>Nova porudžbina na sajtu " + ConfigurationManager.AppSettings["webShopUrl"] + "</strong>");
                body.Append("<br/>");
                body.Append("Porudžbinu možete pogledati na sledećoj stranici: <a href='" + ConfigurationManager.AppSettings["webshopUrl"] + "/" + ConfigurationManager.AppSettings["webshopAdminUrl"] + "/order.aspx?orderID=" + orderID + "'>Porudžbine</a>");

                body.Append("<br/><br/><table width='100%' border='0' cellspacing='0' style='font-family:verdana;font-size:0.9em'>");
                body.Append("<tr bgcolor='#cccccc'>");
                body.Append("<th align='center' style='width:50px'>Rbr</th><th>Naziv</th><th style='width:100px'>Količina</th><th style='width:100px'>Cena</th><th style='width:100px'>Ukupno</th>");
                body.Append("</tr>");
                double ukupno = 0;

                for (int i = 0; i < order.Items.Count; i++)
                {
                    ukupno += order.Items[i].UserPrice * order.Items[i].Quantity;
                    body.Append("<tr height='20px' valign='middle'>");
                    body.Append("<td align='center'>" + (i + 1).ToString() + "</td>" + "" +
                        "<td>" + "<a href='" + ConfigurationManager.AppSettings["webShopUrl"] + "/" + order.Items[i].Product.Url + "' style='color:#d3232e'>" + order.Items[i].Product.Brand.Name + " " + order.Items[i].Product.Name.ToString() + ", " + order.Items[i].Product.Description + ", " + order.Items[i].Product.Code + "</a>" + "</td><td align='right'>" + order.Items[i].Quantity.ToString() + "</td><td align='right'>" + string.Format("{0:N2}", order.Items[i].UserPrice) + "</td><td align='right'>" + string.Format("{0:N2}", order.Items[i].Quantity * order.Items[i].UserPrice) + "</td>");
                    body.Append("</tr>");
                }
                body.Append("</table>");
                body.Append("<div style='text-align:right;'>");
                body.Append("<br/>");
                body.Append("Iznos: " + string.Format("{0:N2}", ukupno));
                body.Append("<br />");
                //double userDiscountValue = (discountTypeID == 1) ? ukupno * userDiscount / 100 : userDiscount;
                body.Append("Popust: " + string.Format("{0:N2}", order.UserDiscountValue));
                body.Append("<br/>");
                body.Append("Ukupno: " + string.Format("{0:N2}", ukupno - order.UserDiscountValue));
                body.Append("<br />");

                //body.Append("Dostava: " + 
                    //string.Format("{0:N2}", ukupno > double.Parse(ConfigurationManager.AppSettings["freeDeliveryTotalValue"]) 
                        //? 0 : double.Parse(ConfigurationManager.AppSettings["deliveryCost"])));

                body.Append("Dostava: " +
                    string.Format("{0:N2}", ukupno > settings.FreeDeliveryTotalValue
                        ? 0 : settings.DeliveryCost));
                body.Append("<br />");

                //body.Append("Ukupno sa dostavom: "
                    //+ string.Format("{0:N2}", ukupno - order.UserDiscountValue + 
                        //(ukupno > double.Parse(ConfigurationManager.AppSettings["freeDeliveryTotalValue"]) 
                            //? 0 : double.Parse(ConfigurationManager.AppSettings["deliveryCost"]))));

                body.Append("Ukupno sa dostavom: "
                    + string.Format("{0:N2}", ukupno - order.UserDiscountValue +
                    (ukupno > settings.FreeDeliveryTotalValue
                        ? 0 : settings.DeliveryCost)));
                body.Append("</div>");
                if(order.UserDiscountValue > 0)
                    body.Append("<p>Odobren je popust u iznosu od: " + string.Format("{0:N2}", order.UserDiscountValue) + " dinara</p");
                body.Append("<br/>");
                if (order.Lastname != string.Empty || order.Firstname != string.Empty)
                    body.Append("<p><strong>Prezime i ime: </strong>" + order.Lastname + " " + order.Firstname + "</p>");
                if (order.Name != string.Empty)
                {
                    body.Append("<p><strong>Naziv: </strong>" + order.Name + "</p>");
                    body.Append("<p><strong>PIB: </strong>" + order.Pib + "</p>");
                }
                body.Append("<p><strong>Adresa: </strong>" + order.Address + " " + order.Code + " " + order.City + " " + order.Zip + "</p>");
                body.Append("<p><strong>Telefon: </strong>" + order.Phone + "</p>");
                body.Append("<p><strong>Način plaćanja: </strong>" + order.Payment.Name + "</p>");
                body.Append("<p><strong>Način preuzimanja: </strong>" + order.Delivery.Name + "</p>");
                body.Append("<p><strong>Napomena: </strong>" + order.Comment + "</p>");

                mail.Body = body.ToString();
                mail.IsBodyHtml = true;
                mail.BodyEncoding = Encoding.UTF8;

                AlternateView plainView = AlternateView.CreateAlternateViewFromString("Nova porudžbina na sajtu", null, "text/plain");
                AlternateView htmlView = AlternateView.CreateAlternateViewFromString(mail.Body, null, "text/html");
                mail.AlternateViews.Add(plainView);
                mail.AlternateViews.Add(htmlView);
                mail.Body = string.Empty;
                mail.Headers.Add("Message-Id", "<" + Guid.NewGuid().ToString() + "@" + ConfigurationManager.AppSettings["webShopDomain"] + ">");

                SmtpClient smtp = getSmtp(ConfigurationManager.AppSettings["infoEmail"].ToString(), "infoEmail");
                //MimeMailer smtp = getAegisSmtp(ConfigurationManager.AppSettings["infoEmail"].ToString(), "infoEmail");
                smtp.Send(mail);
            }
            catch(Exception ex)
            {
                throw new BLException("Nije moguće poslati mail", ex);
            }
        }

        public static void SendMessage(string email, string subject, string body)
        {
            try
            {
                MailMessage mail = new MailMessage();
                //MimeMailMessage mail = new MimeMailMessage();
                mail.From = new MailAddress(ConfigurationManager.AppSettings["infoEmail"].ToString());
                if(ConfigurationManager.AppSettings["webShopDestinationEmail"] == null || ConfigurationManager.AppSettings["webShopDestinationEmail"] == string.Empty)
                    mail.To.Add(new MailAddress(ConfigurationManager.AppSettings["infoEmail"]));
                else
                    foreach(string destinationEmail in ConfigurationManager.AppSettings["webShopDestinationEmail"].Split(','))
                    {
                        mail.To.Add(destinationEmail);
                    }
                mail.ReplyToList.Add(new MailAddress(email));
                mail.Subject = subject;
                mail.Body = body;

                AlternateView plainView = AlternateView.CreateAlternateViewFromString(body, null, "text/plain");
                AlternateView htmlView = AlternateView.CreateAlternateViewFromString(mail.Body, null, "text/html");
                mail.AlternateViews.Add(plainView);
                mail.AlternateViews.Add(htmlView);
                mail.Body = string.Empty;
                mail.Headers.Add("Message-Id", "<" + Guid.NewGuid().ToString() + "@" + ConfigurationManager.AppSettings["webShopDomain"] + ">");


                SmtpClient smtp = getSmtp(ConfigurationManager.AppSettings["infoEmail"].ToString(), "infoEmail");
                //MimeMailer smtp = getAegisSmtp(ConfigurationManager.AppSettings["infoEmail"].ToString(), "infoEmail");
                //ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                //smtp.SendCompleted += Smtp_SendCompleted;
                //smtp.SendMailAsync(mail);
                smtp.Send(mail);
            }
            catch(Exception ex)
            {
                throw new BLException("Nije moguće poslati mail", ex);
            }
        }

        private static void Smtp_SendCompleted(object sender, AsyncCompletedEventArgs e)
        {
            //throw new NotImplementedException();
            var response = e;
        }

        public static SmtpClient getErrorSmtp()
        {
            SmtpClient smtp = new SmtpClient();
            NetworkCredential networkCredential = new NetworkCredential(ConfigurationManager.AppSettings["errorEmailFrom"].ToString(), ConfigurationManager.AppSettings["errorEmailFromPassword"].ToString());
            smtp.UseDefaultCredentials = false;
            smtp.Credentials = networkCredential;
            smtp.Host = ConfigurationManager.AppSettings["errorEmailFromSmtp"].ToString();
            smtp.Port = int.Parse(ConfigurationManager.AppSettings["errorEmailFromSmtpPort"]);
            smtp.EnableSsl = bool.Parse(ConfigurationManager.AppSettings["errorEmailFromSmtpSsl"]);

            return smtp;
        }

        public static void SendOrderStatusUpdate(string email, string name, string orderNumber, DateTime date, string status)
        {
            try
            {
                MailMessage mail = new MailMessage();
                //MimeMailMessage mail = new MimeMailMessage();
                mail.From = new MailAddress(ConfigurationManager.AppSettings["infoEmail"], ConfigurationManager.AppSettings["companyName"]);
                mail.To.Add(new MailAddress(email));
                mail.Subject = ConfigurationManager.AppSettings["companyName"] + " - Status porudžbine";
                StringBuilder body = new StringBuilder();
                body.Append("<p>Poštovani, " + name + "</p>");
                body.Append("<br/>");
                body.Append("<p>Status vaše porudžbine sa brojem: <strong>" + orderNumber + "</strong> od: <strong>" + date.ToShortDateString() + "</strong> je izmenjen na: </p>");
                body.Append("<br/>");
                body.Append("<div style='width=100%;background-color:#f0f000;padding-top:0.5em;padding-bottom:0.5em;padding-left:0.5em;padding-right:0.5em;color:#333333;text-align:center;font-size:18px;margin:1em;height:2em'><strong>" + status.ToUpper() + "</strong></div>");
                body.Append("<br/>");
                body.Append("<p>Vaša online prodavnica <a href='" + ConfigurationManager.AppSettings["webShopUrl"] + "'>" + ConfigurationManager.AppSettings["companyName"] + "</a></p>");

                mail.IsBodyHtml = true;
                mail.Body = body.ToString();

                SmtpClient smtp = getSmtp(ConfigurationManager.AppSettings["infoEmail"], "infoEmail");
                //MimeMailer smtp = getAegisSmtp(ConfigurationManager.AppSettings["infoEmail"], "infoEmail");
                smtp.Send(mail);
            }
            catch(Exception ex)
            {
                throw new BLException("Nije moguće poslati mail", ex);
            }
        }

        public static void SendOrderDiscountGrantedNotification(Order order)
        {
            try
            {
                MailMessage mail = new MailMessage();
                //MimeMailMessage mail = new MimeMailMessage();
                mail.From = new MailAddress(ConfigurationManager.AppSettings["infoEmail"], ConfigurationManager.AppSettings["companyName"]);
                mail.To.Add(new MailAddress(order.Email));
                mail.Subject = "Odobren popust";
                StringBuilder body = new StringBuilder();
                body.Append("<p>Poštovani " + order.Firstname + " " + order.Lastname + "</p>");
                body.Append("<br/>");
                body.Append("<p>Odobren Vam je popust u iznosu od: <strong>" + string.Format("{0:N2}", order.UserDiscountValue) + " dinara</strong> za porudžbinu broj: " + order.Code + " od " + order.Date + "</p>");
                body.Append("<br/>");
                body.Append("<p>Vaša online prodavnica <a href='" + ConfigurationManager.AppSettings["webShopUrl"] + "'>" + ConfigurationManager.AppSettings["companyName"] + "</a></p>");

                mail.IsBodyHtml = true;
                mail.Body = body.ToString();

                SmtpClient smtp = getSmtp(ConfigurationManager.AppSettings["infoEmail"], "infoEmail");
                //MimeMailer smtp = getAegisSmtp(ConfigurationManager.AppSettings["infoEmail"], "infoEmail");
                smtp.Send(mail);
            }
            catch(Exception ex)
            {
                throw new BLException("Nije moguće poslati mail", ex);
            }
        }

        private static SslMode DetectSslType()
        {
            var mailer = new MimeMailer(ConfigurationManager.AppSettings["smtp"], int.Parse(ConfigurationManager.AppSettings["smtpPort"]))
            {
                User = ConfigurationManager.AppSettings["infoEmail"],
                Password = ConfigurationManager.AppSettings["infoEmailPassword"],
                AuthenticationMode = AuthenticationType.Base64,
                //EnableImplicitSsl = true,
                SslType = SslMode.Auto
            };

            //mailer.
            mailer.SendCompleted += Mailer_SendCompleted;
            return mailer.DetectSslMode();
        }

        private static void Mailer_SendCompleted(object sender, AsyncCompletedEventArgs e)
        {
            throw new NotImplementedException();
        }

        public static string CapitalizeFirstLetter(string value)
        {
            if (value == null || value.Length == 0)
                return value;
            return value[0].ToString().ToUpper() + value.Substring(1).ToLower();
        }
    }
}
