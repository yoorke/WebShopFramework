using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Configuration;
using System.Web;
using System.Text.RegularExpressions;

namespace eshopBL
{
    public class NotificationHandler
    {
        private string loadTemplate(string name)
        {
            using (TextReader tr = new StreamReader(HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings["emailTemplatesPath"] + "/" + name)))
            {
                return tr.ReadToEnd();
            }
        }

        public string GenerateContent(Dictionary<string, string> replaceTags, string templateName)
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
    }
}
