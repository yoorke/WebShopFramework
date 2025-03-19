using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Web;
using System.Data.SqlClient;
using System.Net.Mail;
using System.Configuration;
using System.Threading;

namespace eshopUtilities
{
    public class ErrorLog
    {
        public static void LogError(Exception ex, string rawUrl = "", string userHostAddress = "", string url = "", string serverPath = "")
        {
            if(string.IsNullOrWhiteSpace(serverPath))
            {
                serverPath = HttpContext.Current.Server.MapPath("~/");
            }

            int code = 0;
            string message = string.Empty;

            if (ex is SqlException)
                code = ((SqlException)ex).Number;
            message = getMessage(ex);

            try
            { 
                using (StreamWriter sw = new StreamWriter($"{serverPath}/log/{DateTime.Now.ToString("ddMMyyyy")}-error.log", true))
                {
                    sw.WriteLine(DateTime.Now.ToUniversalTime().ToString() + " - " + code.ToString() + " - " + message + " " + Environment.NewLine + rawUrl +  Environment.NewLine + userHostAddress + Environment.NewLine + url);
                }

                if (bool.Parse(ConfigurationManager.AppSettings["sendErrorEmail"]))
                    sendMail(message, rawUrl, userHostAddress, url);
            }
            catch(Exception exx)
            {
                if(exx is IOException)
                { 
                    Thread.Sleep(1000);
                    LogError(ex, rawUrl, userHostAddress, url, serverPath);
                }
            }


            //if (ex.InnerException != null)
            //LogError(ex.InnerException);
        }

        public static void LogMessage(string message, string serverPath = "")
        {
            if(string.IsNullOrWhiteSpace(serverPath))
            {
                serverPath = HttpContext.Current.Server.MapPath("~/");
            }

            try
            { 
                using (StreamWriter sw = new StreamWriter($"{serverPath}/log/{ DateTime.Now.ToString("ddMMyyyy")}.log", true))
                    sw.WriteLine(DateTime.Now.ToUniversalTime().ToString() + " - " + message);
            }
            catch(Exception ex)
            {
                if(ex is IOException)
                { 
                    Thread.Sleep(100);
                    LogMessage(message, serverPath);
                }
            }
        }

        private static void sendMail(string message, string rawUrl, string userHostAddress, string url)
        {
            try
            { 
                MailMessage mailMessage = new MailMessage();
                mailMessage.From = new MailAddress(ConfigurationManager.AppSettings["errorEmailFrom"]);
                mailMessage.To.Add(new MailAddress(ConfigurationManager.AppSettings["errorEmailTo"]));
                mailMessage.Subject = ConfigurationManager.AppSettings["CompanyName"] + " - Error";
                mailMessage.BodyEncoding = Encoding.UTF8;
                mailMessage.IsBodyHtml = true;
                mailMessage.Body = message;
                mailMessage.Body += "<br/>" + Environment.NewLine + "<strong>Raw url: </strong>" + rawUrl;
                mailMessage.Body += "<br/>" + Environment.NewLine + "<strong>User host address: </strong>" + userHostAddress;
                mailMessage.Body += "<br/>" + Environment.NewLine + "<strong>Url: </strong>" + url;

                SmtpClient smtp = Common.getErrorSmtp();
                smtp.Send(mailMessage);
            }
            catch(Exception ex)
            {
                LogMessage(ex.Message);
            }
        }

        private static string getMessage(Exception ex)
        {

            if (ex.InnerException == null)
                return ex.Message;
            else return ex.Message + Environment.NewLine + "<strong>Stack trace: </strong>" + ex.StackTrace + Environment.NewLine + "<strong>Message: </strong>" + getMessage(ex.InnerException);
        }
    }
}
