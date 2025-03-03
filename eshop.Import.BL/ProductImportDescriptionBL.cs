using eshop.Import.BL.Interfaces;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace eshop.Import.BL
{
    public class ProductImportDescriptionBL : IProductImportDescriptionBL
    {
        public ProductImportDescriptionBL()
        {

        }

        private string removeTag(string description, string tag)
        {
            if(!description.Contains("<table"))
            {
                return description;
            }

            return description.Replace(tag, "");
        }

        public Dictionary<string, string> GetDescriptionAttributes(string description)
        {
            Dictionary<string, string> attributes = new Dictionary<string, string>();
            HtmlDocument htmlDoc = new HtmlDocument();
            int tdCounter;

            if(!description.Contains("<table"))
            {
                return attributes;
            }

            htmlDoc.LoadHtml(description);

            foreach (var tr in htmlDoc.DocumentNode.SelectNodes("//table//tr"))
            {
                tdCounter = 0;
                string name = string.Empty;
                string value = string.Empty;
                foreach (var td in tr.SelectNodes("td"))
                {
                    if (tdCounter == 0)
                        name = td.InnerText;
                    else if (tdCounter == 1)
                    { 
                        value = td.InnerText;
                        attributes.Add(name, value);
                    }
                    tdCounter++;
                }
            }

            return attributes;
        }

        public string InsertClassToDescription(string description)
        {
            description = removeTag(description, "<br>");
            if(!description.Contains("<table"))
            {
                return description;
            }

            Regex regex = new Regex("<table.*?>");
            string replace = "<table class='table table-condensed table-bordered table-hover table-striped'>";
            return regex.Replace(description, replace);
        }
    }
}
