using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Mail;
using System.Net;
using System.Configuration;
using eshopUtilities;

namespace eshopBL
{
    public class MailBL
    {
        private SmtpClient getSmtp(string type)
        {
            SmtpClient smtp = new SmtpClient();
            NetworkCredential networkCredentials = new NetworkCredential(ConfigurationManager.AppSettings[$"{type}Email"], ConfigurationManager.AppSettings[$"{type}EmailPassword"]);
            smtp.UseDefaultCredentials = false;
            smtp.Credentials = networkCredentials;
            smtp.Host = ConfigurationManager.AppSettings["smtp"];
            smtp.Port = int.Parse(ConfigurationManager.AppSettings["smtpPort"]);
            smtp.EnableSsl = bool.Parse(ConfigurationManager.AppSettings["smtpSsl"]);

            return smtp;
        }

        public void SendMail(string email, string subject, string content, string plainContent)
        {
            try
            {
                ErrorLog.LogMessage($"Sending mail to:{email}. Subject: {subject}");
                MailMessage message = new MailMessage();
                message.From = new MailAddress(ConfigurationManager.AppSettings["infoEmail"], ConfigurationManager.AppSettings["companyName"]);
                message.To.Add(new MailAddress(email));
                message.Subject = subject;
                message.BodyEncoding = Encoding.UTF8;
                message.IsBodyHtml = true;
                //message.Body = content;

                AlternateView plainView = AlternateView.CreateAlternateViewFromString(plainContent, null, "text/plain");
                AlternateView htmlView = AlternateView.CreateAlternateViewFromString(content, null, "text/html");

                message.AlternateViews.Add(plainView);
                message.AlternateViews.Add(htmlView);

                message.Headers.Add("Message-Id", $"<{Guid.NewGuid().ToString()}@{ConfigurationManager.AppSettings["webShopDomain"]}>");

                SmtpClient smtp = getSmtp("info");

                smtp.Send(message);
                ErrorLog.LogMessage("Mail sent");
            }
            catch(Exception ex)
            {
                ErrorLog.LogError(ex);
                throw new BLException("Nije moguće poslati mail", ex);
            }
        }
    }
}
