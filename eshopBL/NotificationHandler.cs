using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Configuration;
using System.Web;
using System.Text.RegularExpressions;
using eshopBE;
using eshop.PaymentProcessor.BE;
using System.Globalization;

namespace eshopBL
{
    public class NotificationHandler
    {
        private string loadTemplate(string name)
        {
            using (TextReader tr = new StreamReader(HttpContext.Current.Server.MapPath("~/" + ConfigurationManager.AppSettings["emailTemplatesPath"] + "/" + name)))
            {
                return tr.ReadToEnd();
            }
        }

        public string generateContent(Dictionary<string, string> replaceTags, string templateName)
        {
            Regex regex;
            string template = loadTemplate(templateName);
            foreach(KeyValuePair<string, string> tag in replaceTags)
            {
                regex = new Regex("\\[" + tag.Key + "\\]");
                template = regex.Replace(template, tag.Value);
            }

            return template;
        }

        private string getHeader()
        {
            Dictionary<string, string> replaceTags = new Dictionary<string, string>();

            replaceTags.Add("LOGO-URL", ConfigurationManager.AppSettings["logoUrl"]);
            replaceTags.Add("COMPANY-NAME", ConfigurationManager.AppSettings["companyName"]);
            replaceTags.Add("WEBSHOP-URL", ConfigurationManager.AppSettings["webShopDomain"]);
            replaceTags.Add("WEBSHOP-DESCRIPTION", ConfigurationManager.AppSettings["companyName"]);
            replaceTags.Add("TITLE", "Informacija o plaćanju");

            return generateContent(replaceTags, "headerTemplate.html");
        }

        private string getFooter()
        {
            Dictionary<string, string> replaceTags = new Dictionary<string, string>();
            replaceTags.Add("COMPANY-NAME", ConfigurationManager.AppSettings["companyName"]);
            replaceTags.Add("WEBSHOP-NAME", ConfigurationManager.AppSettings["companyName"]);
            replaceTags.Add("WEBSHOP-ADDRESS", ConfigurationManager.AppSettings["companyAddress"]);
            replaceTags.Add("WEBSHOP-PHONE", ConfigurationManager.AppSettings["companyPhone"]);
            replaceTags.Add("WEBSHOP-EMAIL", ConfigurationManager.AppSettings["infoEmail"]);

            return generateContent(replaceTags, "footerTemplate.html");
        }

        public void SendOrderConfirmationMail(string email, string name, Order order, Settings settings)
        {
            Dictionary<string, string> replaceTags = new Dictionary<string, string>();

            double total = order.Items.Sum(item => item.UserPrice * item.Quantity);

            replaceTags.Add("CUSTOMER-NAME", order.Firstname + " " + order.Lastname);
            replaceTags.Add("TOTAL", string.Format("{0:N2}", total));
            replaceTags.Add("DISCOUNT", string.Format("{0:N2}", order.UserDiscountValue));
            replaceTags.Add("TOTAL-VALUE", string.Format("{0:N2}", total - order.UserDiscountValue));
            replaceTags.Add("DELIVERY", getDeliveryValue(total - order.UserDiscountValue, settings));
            replaceTags.Add("TOTAL-WITH-DELIVERY", getTotalWithDeliveryValue(total - order.UserDiscountValue, settings));
            replaceTags.Add("FIRST-NAME", order.Firstname);
            replaceTags.Add("LAST-NAME", order.Lastname);
            replaceTags.Add("TAX-NUMBER", order.Pib);
            replaceTags.Add("ADDRESS", order.Address + " " + order.Code + " " + order.City + " " + order.Zip);
            replaceTags.Add("PHONE", order.Phone);
            replaceTags.Add("PAYMENT", order.Payment.Name);
            replaceTags.Add("DELIVERY-NAME", order.Delivery.Name);
            replaceTags.Add("COMMENT", order.Comment);
            replaceTags.Add("ORDER-ITEMS", getOrderItemsContent(order.Items));
            replaceTags.Add("FULL-NAME", order.Firstname + " " + order.Lastname + " " + order.Name);

            StringBuilder content = new StringBuilder();
            content.Append(getHeader());

            content.Append(generateContent(replaceTags, "orderConfirmationTemplate.html"));

            content.Append(getFooter());

            new MailBL().SendMail(order.Email, "Potvrda porudžbine", content.ToString(), $"Kreirana porudžbenica broj {order.Code} na {ConfigurationManager.AppSettings["webShopDomain"]} online prodavnici u iznosu od {string.Format("{0:N2}", order.TotalValue)}");

        }

        private string getOrderItemsContent(List<OrderItem> items)
        {
            StringBuilder content = new StringBuilder();
            int i = 1;
            foreach (OrderItem item in items)
                content.Append(getOrderItemContent(item, i++));

            return content.ToString();
        }

        private string getOrderItemContent(OrderItem item, int index)
        {
            Dictionary<string, string> replaceTags = new Dictionary<string, string>();
            replaceTags.Add("INDEX", index.ToString());
            replaceTags.Add("NAME", item.Product.Brand.Name + " " + item.Product.Name);
            replaceTags.Add("QUANTITY", item.Quantity.ToString());
            replaceTags.Add("PRICE", string.Format("{0:N2}", item.UserPrice));
            replaceTags.Add("TOTAL", string.Format("{0:N2}", item.Quantity * item.UserPrice));
            replaceTags.Add("PRODUCT-URL", ConfigurationManager.AppSettings["webShopUrl"] + item.Product.Url);

            return generateContent(replaceTags, "orderItemTemplate.html");
        }

        private string getDeliveryValue(double totalValue, Settings settings)
        {
            return bool.Parse(ConfigurationManager.AppSettings["calculateDelivery"])
                ? string.Format("{0:N2}", totalValue > settings.FreeDeliveryTotalValue ? 0 : settings.DeliveryCost)
                : totalValue > settings.FreeDeliveryTotalValue ? "0,00" : "Po cenovniku kurirske službe";
        }

        private string getTotalWithDeliveryValue(double totalValue, Settings settings)
        {
            return bool.Parse(ConfigurationManager.AppSettings["calculateDelivery"])
                ? string.Format("{0:N2}", totalValue + double.Parse(getDeliveryValue(totalValue, settings)))
                : totalValue + " + cena dostave";
        }



        public void SendOrderConfirmationMail(Order order, Settings settings)
        {
            Dictionary<string, string> replaceTags = new Dictionary<string, string>();

            replaceTags.Add("HEADER", getHeaderContent());
            replaceTags.Add("LOGO", getLogoContent());
            replaceTags.Add("TITLE", getTitleContent());
            replaceTags.Add("TABLE-HEADER", getTableHeaderContent());
            replaceTags.Add("STYLE", getStyleContent());

            replaceTags.Add("ITEMS", getItemsContent(order.Items));

            replaceTags.Add("TOTALS", getTotalsContent(order));
            replaceTags.Add("SUPPORT", getSupportContent());
            replaceTags.Add("FOOTER", getFooterContent());

            StringBuilder content = new StringBuilder();
            content.Append(generateContent(replaceTags, "emailDocumentTemplate.html"));

            new MailBL().SendMail(order.Email, "Potvrda porudžbine", content.ToString(), $"Kreirana porudžbenica broj {order.Code} na {ConfigurationManager.AppSettings["webShopDomain"]} online prodavnici u iznosu od {string.Format("{0:N2}", order.TotalValue)}");
        }

        public void SendSuccessPaymentResponse(Order order, Settings settings, PaymentResponse paymentResponse)
        {
            Dictionary<string, string> replaceTags = new Dictionary<string, string>();

            StringBuilder content = new StringBuilder();

            replaceTags.Add("WEBSHOP-URL", ConfigurationManager.AppSettings["webShopDomain"]);
            replaceTags.Add("WEBSHOP-DESCRIPTION", ConfigurationManager.AppSettings["companyName"]);
            replaceTags.Add("NAME", order.Firstname);
            //replaceTags.Add("AMOUNT", order.TotalValue.ToString("{0:N2}", new CultureInfo("sr-Latn-RS")));
            replaceTags.Add("AMOUNT", string.Format("{0:N2}", double.Parse(paymentResponse.Amount, new CultureInfo("en-US"))));
            replaceTags.Add("ORDER-ID", order.Code);
            replaceTags.Add("AUTH-CODE", paymentResponse.AuthCode);
            replaceTags.Add("RESPONSE", paymentResponse.Response);
            replaceTags.Add("PROC-RETURN-CODE", paymentResponse.ProcReturnCode);
            replaceTags.Add("TRANSACTION-DATE", paymentResponse.Date.ToString("dd.MM.yyyy HH:mm"));
            replaceTags.Add("MD-STATUS", paymentResponse.Md);

            content.Append(getHeader());
            content.Append(generateContent(replaceTags, "paymentConfirmationTemplate.html"));
            content.Append(getFooter());

            new MailBL().SendMail(order.Email, $"Potvrda plaćanja za porudžbinu broj {order.Code}", content.ToString(), $"Plaćanje u iznosu od {string.Format("{0:N2}", double.Parse(paymentResponse.Amount, new CultureInfo("en-US")))} dinara uspešno izvršeno");
        }

        public void SendFailPaymentResponse(Order order, Settings settings, PaymentResponse paymentResponse)
        {
            Dictionary<string, string> replaceTags = new Dictionary<string, string>();

            StringBuilder content = new StringBuilder();

            replaceTags.Add("NAME", order.Firstname);
            replaceTags.Add("ORDER-ID", order.Code);

            content.Append(getHeader());
            content.Append(generateContent(replaceTags, "paymentFailedTemplate.html"));
            content.Append(getFooter());

            new MailBL().SendMail(order.Email, $"Plaćanje neuspešno za porudžbinu broj {order.Code}", content.ToString(), $"Plaćanje nije uspešno izvršeno");
        }

        private string getHeaderContent()
        {
            Dictionary<string, string> replaceTags = new Dictionary<string, string>();            

            return generateContent(replaceTags, "emailHeaderTemplate.html");
        }

        private string getLogoContent()
        {
            Dictionary<string, string> replaceTags = new Dictionary<string, string>();
            replaceTags.Add("LOGO-URL", ConfigurationManager.AppSettings["logoUrl"]);
            replaceTags.Add("MY-ACCOUNT-URL", $"{ConfigurationManager.AppSettings["webShopUrl"]}/moj-nalog");

            return generateContent(replaceTags, "emailLogoTemplate.html");
        }

        private string getTitleContent()
        {
            Dictionary<string, string> replaceTags = new Dictionary<string, string>();

            return generateContent(replaceTags, "emailTitleTemplate.html");
        }

        private string getTableHeaderContent()
        {
            Dictionary<string, string> replaceTags = new Dictionary<string, string>();

            return generateContent(replaceTags, "emailTableHeaderTemplate.html");
        }

        private string getItemsContent(List<OrderItem> items)
        {
            StringBuilder content = new StringBuilder();
            int i = 1;

            foreach (var item in items)
            {
                content.Append(getItemContent(item, i++));
            }

            return content.ToString();
        }

        private string getItemContent(OrderItem item, int index)
        {
            Dictionary<string, string> replaceTags = new Dictionary<string, string>();

            return generateContent(replaceTags, "emailTableItemTemplate.html");
        }

        private string getTotalsContent(Order order)
        {
            Dictionary<string, string> replaceTags = new Dictionary<string, string>();

            return generateContent(replaceTags, "emailTotalsTemplate.html");
        }

        private string getSupportContent()
        {
            Dictionary<string, string> replaceTags = new Dictionary<string, string>();

            return generateContent(replaceTags, "emailSupportTemplate.html");
        }

        private string getFooterContent()
        {
            Dictionary<string, string> replaceTags = new Dictionary<string, string>();

            return generateContent(replaceTags, "emailFooterTemplate.html");
        }

        private string getStyleContent()
        {
            Dictionary<string, string> replaceTags = new Dictionary<string, string>();

            return generateContent(replaceTags, "emailStyleTemplate.html");
        }
    }
}
