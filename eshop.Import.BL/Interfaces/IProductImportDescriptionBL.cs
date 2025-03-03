using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eshop.Import.BL.Interfaces
{
    public interface IProductImportDescriptionBL
    {
        //string RemoveTag(string description, string tag);
        Dictionary<string, string> GetDescriptionAttributes(string description);
        string InsertClassToDescription(string description);
    }
}
