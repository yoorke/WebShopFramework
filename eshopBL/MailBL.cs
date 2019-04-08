using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Mail;
using System.Net;
using System.Configuration;

namespace eshopBL
{
    public class MailBL
    {
        private SmtpClient getSmtp()
        {
            SmtpClient smtp = new SmtpClient();
            NetworkCredential networkCredentials = new NetworkCredential(ConfigurationManager.AppSettings["infoEmail"], ConfigurationManager.AppSettings["infoEmailPassword"]);
            smtp.UseDefaultCredentials = false;
            smtp.Credentials = networkCredentials;
            smtp.Host = ConfigurationManager.AppSettings["smtp"];
            smtp.Port = int.Parse(ConfigurationManager.AppSettings["smtpPort"]);
            smtp.EnableSsl = bool.Parse(ConfigurationManager.AppSettings["smtpSsl"]);

            return smtp;
        }

        public void SendMail(string email, string subject, string content)
        {
            try
            {
                MailMessage message = new MailMessage();
                message.From = new MailAddress(ConfigurationManager.AppSettings["infoEmail"], ConfigurationManager.AppSettings["companyName"]);
                message.To.Add(new MailAddress(email));
                message.Subject = subject;
                message.BodyEncoding = Encoding.UTF8;
                message.IsBodyHtml = true;
                message.Body = content;

                SmtpClient smtp = getSmtp();

                smtp.Send(message);
            }
            catch(Exception ex)
            {

            }
        }
    }
}
