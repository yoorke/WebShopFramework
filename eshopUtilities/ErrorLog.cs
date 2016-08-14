using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Web;
using System.Data.SqlClient;
using System.Net.Mail;
using System.Configuration;

namespace eshopUtilities
{
    public class ErrorLog
    {
        public static void LogError(Exception ex, string rawUrl = "", string userHostAddress = "", string url = "")
        {
            int code = 0;
            string message = string.Empty;

            if (ex is SqlException)
                code = ((SqlException)ex).Number;
            message = getMessage(ex);

            using (StreamWriter sw = new StreamWriter(HttpContext.Current.Server.MapPath("/log/error.log"), true))
                sw.WriteLine(DateTime.Now.ToUniversalTime().ToString() + " - " + code.ToString() + " - " + message + " " + Environment.NewLine + rawUrl +  Environment.NewLine + userHostAddress + Environment.NewLine + url);
            sendMail(message, rawUrl, userHostAddress, url);


            //if (ex.InnerException != null)
                //LogError(ex.InnerException);
        }

        public static void LogMessage(string message)
        {
            using (StreamWriter sw = new StreamWriter(HttpContext.Current.Server.MapPath("/log/error.log"), true))
                sw.WriteLine(DateTime.Now.ToUniversalTime().ToString() + " - " + message);
        }

        private static void sendMail(string message, string rawUrl, string userHostAddress, string url)
        {
            MailMessage mailMessage = new MailMessage();
            mailMessage.From = new MailAddress(ConfigurationManager.AppSettings["errorEmail"]);
            mailMessage.To.Add(new MailAddress(ConfigurationManager.AppSettings["errorEmail"]));
            mailMessage.Subject = "Error";
            mailMessage.BodyEncoding = Encoding.UTF8;
            mailMessage.Body = message;
            mailMessage.Body += Environment.NewLine + rawUrl;
            mailMessage.Body += Environment.NewLine + userHostAddress;
            mailMessage.Body += Environment.NewLine + url;

            SmtpClient smtp = Common.getErrorSmtp();
            //smtp.Send(mailMessage);
        }

        private static string getMessage(Exception ex)
        {

            if (ex.InnerException == null)
                return ex.Message;
            else return ex.Message + Environment.NewLine + ex.StackTrace + Environment.NewLine + getMessage(ex.InnerException);
        }
    }
}
