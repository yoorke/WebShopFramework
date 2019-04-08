using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Configuration;
using System.Web;
using System.Text.RegularExpressions;
using eshopBE;

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

            return generateContent(replaceTags, "headerTemplate.html");
        }

        private string getFooter()
        {
            Dictionary<string, string> replaceTags = new Dictionary<string, string>();
            replaceTags.Add("COMPANY-NAME", ConfigurationManager.AppSettings["companyName"]);

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

            new MailBL().SendMail(order.Email, "Potvrda porudžbine", content.ToString());

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
    }
}
