using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eshopDL;
using eshopBE;
using System.Data;

namespace eshopBL
{
    public class AttributeBL
    {
        public List<eshopBE.Attribute> GetAllAttributes()
        {
            AttributeDL attributeDL = new AttributeDL();
            return attributeDL.GetAttributes();
        }

        public List<eshopBE.Attribute> GetAttributesForCategory(int categoryID)
        {
            AttributeDL attributeDL = new AttributeDL();
            return attributeDL.GetAttributesForCategory(categoryID);
        }

        public eshopBE.Attribute GetAttribute(string name)
        {
            AttributeDL attributeDL = new AttributeDL();
            return attributeDL.GetAttribute(name);
        }

        public eshopBE.Attribute GetAttribute(int attributeID)
        {
            AttributeDL attributeDL = new AttributeDL();
            return attributeDL.GetAttribute(attributeID);
        }

        public int SaveAttribute(eshopBE.Attribute attribute)
        {
            AttributeDL attributeDL = new AttributeDL();
            if (attribute.AttributeID > 0)
                return attributeDL.UpdateAttribute(attribute);
            else
                return attributeDL.SaveAttribute(attribute);
        }

        public int SaveAttributeForCategory(int categoryID, int attributeID)
        {
            AttributeDL attributeDL = new AttributeDL();
            return attributeDL.SaveAttributeForCategory(categoryID, attributeID);
        }

        public int DeleteAttributeForCategory(int categoryID, int attributeID)
        {
            AttributeDL attributeDL = new AttributeDL();
            return attributeDL.DeleteAttributeForCategory(categoryID, attributeID);
        }

        public List<AttributeValue> GetAttributeValues(int attributeID)
        {
            AttributeDL attributeDL = new AttributeDL();
            return attributeDL.GetAttributeValues(attributeID, true);
        }
        
        public int SaveAttributeValue(AttributeValue attributeValue, bool isKimtec)
        {
            AttributeDL attributeDL = new AttributeDL();
            return attributeDL.SaveAttributeValue(attributeValue, isKimtec);
        }

        public int DeleteAttributeValue(int attributeValueID)
        {
            AttributeDL attributeDL = new AttributeDL();
            return attributeDL.DeleteAttributeValue(attributeValueID);
        }

        public int DeleteAttribute(int attributeID)
        {
            AttributeDL attributeDL = new AttributeDL();
            return attributeDL.DeleteAttribute(attributeID);
        }

        public int SetFilter(int categoryID, int attributeID, bool filter)
        {
            AttributeDL attributeDL = new AttributeDL();
            return attributeDL.SetFilter(categoryID, attributeID, filter);
        }

        public int SetIsDescription(int categoryID, int attributeID, bool isDescription)
        {
            AttributeDL attributeDL = new AttributeDL();
            return attributeDL.SetIsDescription(categoryID, attributeID, isDescription);
        }

        public List<eshopBE.Attribute> GetAttributeListForFilter(string categoryUrl)
        {
            AttributeDL attributeDL = new AttributeDL();
            return attributeDL.GetAttributeListForFilter(categoryUrl);
        }

        public int SaveAttributePositionForCategory(int attributeID, int categoryID, int position)
        {
            AttributeDL attributeDL = new AttributeDL();
            return attributeDL.SaveAttributePositionForCategory(attributeID, categoryID, position);
        }

        public DataTable GetAttributesForMapping(int categoryID, int kimtecCategoryID)
        {
            return new AttributeDL().GetAttributesForMapping(categoryID, kimtecCategoryID);
        }

        public int SaveAttributeValueSort(int attributeValueID, int sort)
        {
            return new AttributeDL().SaveAttributeValueSort(attributeValueID, sort);
        }
    }
}
