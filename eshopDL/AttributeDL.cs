using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eshopBE;
using System.Data.SqlClient;
using System.Web.Configuration;
using System.Data;
using eshopUtilities;

namespace eshopDL
{
    public class AttributeDL
    {
        private List<eshopBE.Attribute> GetAttributesList(int attributeID, string name, int categoryID)
        {
            List<eshopBE.Attribute> attributesList = null;
            eshopBE.Attribute attribute;

            using (SqlConnection objConn = new SqlConnection(WebConfigurationManager.ConnectionStrings["eshopConnectionString"].ConnectionString))
            {
                using (SqlCommand objComm = new SqlCommand("SELECT attribute.attributeID, name, isDescription FROM attribute"))
                {
                    try
                    {
                        objConn.Open();
                        objComm.Connection = objConn;

                        if (attributeID > 0)
                        {
                            objComm.CommandText += " WHERE attributeID=@attributeID";
                            objComm.Parameters.Add("@attributeID", SqlDbType.Int).Value = attributeID;
                        }
                        else if (name != string.Empty)
                        {
                            objComm.CommandText += " WHERE name=@name";
                            objComm.Parameters.Add("@name", SqlDbType.NVarChar, 50).Value = name;
                        }
                        else if (categoryID > 0)
                        {
                            objComm.CommandText += " INNER JOIN categoryAttribute ON attribute.attributeID=categoryAttribute.attributeID WHERE categoryID=@categoryID";
                            objComm.Parameters.Add("@categoryID", SqlDbType.Int).Value = categoryID;
                        }

                        objComm.CommandText += " ORDER BY name";

                        using (SqlDataReader reader = objComm.ExecuteReader())
                        {
                            if (reader.HasRows)
                                attributesList = new List<eshopBE.Attribute>();
                            while (reader.Read())
                            {
                                attribute = new eshopBE.Attribute();
                                attribute.AttributeID = reader.GetInt32(0);
                                attribute.Name = reader.GetString(1);
                                attribute.IsDescription = Convert.IsDBNull(reader[2]) == false ? reader.GetBoolean(2) : false;
                                attribute.Filter = false;
                                //attribute.Values = GetAttributeValues(attribute.AttributeID);

                                attributesList.Add(attribute);
                            }
                        }
                    }
                    catch (SqlException ex)
                    {
                        ErrorLog.LogError(ex);
                        throw new DLException("Error while loading attributes list", ex);
                    }
                }
            }

            return attributesList;
        }

        public List<eshopBE.Attribute> GetAttributes()
        {
            return GetAttributesList(-1, string.Empty, -1);
        }

        public List<eshopBE.Attribute> GetAttributesForCategory(int categoryID)
        {
            List<eshopBE.Attribute> attributes = null;
            using (SqlConnection objConn = new SqlConnection(WebConfigurationManager.ConnectionStrings["eshopConnectionString"].ConnectionString))
            {
                using (SqlCommand objComm = new SqlCommand("SELECT attribute.attributeID, name, filter, categoryAttribute.isDescription, position FROM attribute INNER JOIN categoryAttribute ON attribute.attributeID=categoryAttribute.attributeID WHERE categoryID=@categoryID ORDER BY position, name", objConn))
                {
                    objConn.Open();
                    objComm.Parameters.Add("@categoryID", SqlDbType.Int).Value = categoryID;
                    using (SqlDataReader reader = objComm.ExecuteReader())
                    {
                        if (reader.HasRows)
                            attributes = new List<eshopBE.Attribute>();
                        while (reader.Read())
                            attributes.Add(new eshopBE.Attribute(reader.GetInt32(0), reader.GetString(1), reader.GetBoolean(2), Convert.IsDBNull(reader[3]) == false ? reader.GetBoolean(3) : false, (Convert.IsDBNull(reader[4])==false)?reader.GetInt32(4):0));
                    }
                }
            }
            return attributes;
        }

        public List<AttributeValue> GetAttributeValues(int attributeID, bool showNP)
        {
            List<AttributeValue> values = null;

            using (SqlConnection objConn = new SqlConnection(WebConfigurationManager.ConnectionStrings["eshopConnectionString"].ConnectionString))
            {
                using (SqlCommand objComm = new SqlCommand("SELECT attributeValueID, value, attributeID, sort, kimtecValue, (SELECT COUNT(*) FROM attributeValue INNER JOIN productAttributeValue ON attributeValue.attributeValueID = productAttributeValue.attributeValueID WHERE attributeValue.attributeValueID = av.attributeValueID) FROM attributeValue av WHERE attributeID=@attributeID"))
                {
                    try
                    {
                        objConn.Open();
                        objComm.Connection = objConn;
                        objComm.CommandText += (!showNP) ? " AND value<>'NP'" : string.Empty;
                        objComm.Parameters.Add("@attributeID", SqlDbType.Int).Value = attributeID;

                        objComm.CommandText += " ORDER BY sort, value";
                        using (SqlDataReader reader = objComm.ExecuteReader())
                        {
                            if (reader.HasRows)
                                values = new List<AttributeValue>();
                            while (reader.Read())
                                values.Add(new AttributeValue(reader.GetInt32(0), reader.GetString(1), reader.GetInt32(2), (!Convert.IsDBNull(reader[3])) ? reader.GetInt32(3) : 0, !Convert.IsDBNull(reader[4]) ? reader.GetString(4) : string.Empty, !Convert.IsDBNull(reader[5]) ? reader.GetInt32(5): 0));
                        }
                    }
                    catch (SqlException ex)
                    {
                        ErrorLog.LogError(ex);
                        throw new DLException("Error while loading attribute values.", ex);
                    }
                }
            }
            return values;
        }

        public List<AttributeValue> GetAttributeValuesForFilter(int attributeID)
        {
            List<AttributeValue> attributeValues = null;
            using (SqlConnection objConn = new SqlConnection(WebConfigurationManager.ConnectionStrings["eshopConnectionString"].ConnectionString))
            {
                using (SqlCommand objComm = new SqlCommand("SELECT attributeValue.attributeValueID, attributeValue.value, attributeValue.attributeID, COUNT(*), sort FROM attributeValue INNER JOIN productAttributeValue ON attributeValue.attributeValueID=productAttributeValue.attributeValueID WHERE attributeID=@attributeID AND value<>'NP' GROUP BY attributeValue.attributeValueID, attributeValue.value, attributeValue.attributeID, sort HAVING COUNT(*)>0 ORDER BY sort, attributeValue.value", objConn))
                {
                    objConn.Open();
                    
                    objComm.Parameters.Add("@attributeID", SqlDbType.Int).Value = attributeID;
                    using (SqlDataReader reader = objComm.ExecuteReader())
                    {
                        if (reader.HasRows)
                            attributeValues = new List<AttributeValue>();
                        while (reader.Read())
                        {
                            attributeValues.Add(new AttributeValue(reader.GetInt32(0), reader.GetString(1), reader.GetInt32(2), 0, string.Empty, 0));
                        }
                    }
                }
            }
            return attributeValues;
        }

        /*private eshopBE.Attribute GetAttribute(int attributeID, string name)
        {
            eshopBE.Attribute attribute = null;

            using (SqlConnection objConn = new SqlConnection(WebConfigurationManager.ConnectionStrings["eshopConnectionString"].ConnectionString))
                using (SqlCommand objComm = new SqlCommand("SELECT attributeID, name FROM attribute", objConn))
                {
                    objConn.Open();

                    if(attributeID>0)
                    {
                        objComm.CommandText += " WHERE attributeID=@attributeID";
                        objComm.Parameters.Add("@attributeID", SqlDbType.Int).Value = attributeID;
                    }
                    else if (name != string.Empty)
                    {
                        objComm.CommandText += " WHERE name=@name";
                        objComm.Parameters.Add("@name", SqlDbType.NVarChar, 50).Value = name;
                    }

                    using (SqlDataReader reader = objComm.ExecuteReader())
                        while (reader.Read())
                        {
                            attribute = new eshopBE.Attribute();
                            attribute.AttributeID = reader.GetInt32(0);
                            attribute.
                        }
                }*/

        public int SaveAttribute(eshopBE.Attribute attribute)
        {
            int attributeID = 0;
            using (SqlConnection objConn = new SqlConnection(WebConfigurationManager.ConnectionStrings["eshopConnectionString"].ConnectionString))
            {
                using (SqlCommand objComm = new SqlCommand("INSERT INTO attribute (name, isDescription) VALUES (@name, @isDescription);SELECT SCOPE_IDENTITY()", objConn))
                {
                    try
                    {
                        objConn.Open();

                        objComm.Parameters.Add("@name", SqlDbType.NVarChar, 100).Value = attribute.Name;
                        objComm.Parameters.Add("@isDescription", SqlDbType.Bit).Value = attribute.IsDescription;

                        attributeID = int.Parse(objComm.ExecuteScalar().ToString());
                        if (attributeID > 0)
                            SaveAttributeValue(new AttributeValue(-1, "NP", attributeID, 0, string.Empty, 0), false);
                        //SaveAttributeValues(attribute.Values);
                    }
                    catch (SqlException ex)
                    {
                        ErrorLog.LogError(ex);
                        throw new DLException("Error while saving attribute.", ex);
                    }
                }
            }
            return attributeID;
        }

        public int UpdateAttribute(eshopBE.Attribute attribute)
        {
            int status = 0;
            using (SqlConnection objConn = new SqlConnection(WebConfigurationManager.ConnectionStrings["eshopConnectionString"].ConnectionString))
            {
                using (SqlCommand objComm = new SqlCommand("UPDATE attribute SET name=@name, isDescription=@isDescription WHERE attributeID=@attributeID", objConn))
                {
                    try
                    {
                        objConn.Open();

                        objComm.Parameters.Add("@name", SqlDbType.NVarChar, 100).Value = attribute.Name;
                        objComm.Parameters.Add("@isDescription", SqlDbType.Bit).Value = attribute.IsDescription;
                        
                        objComm.Parameters.Add("@attributeID", SqlDbType.Int).Value = attribute.AttributeID;

                        status = objComm.ExecuteNonQuery();
                        //if (status > 0)
                        //SaveAttributeValues(attribute.Values);
                    }
                    catch (SqlException ex)
                    {
                        ErrorLog.LogError(ex);
                        throw new DLException("Error while updating attribute.", ex);
                    }
                }
            }
            return status;
        }

        /*private void SaveAttributeValues(List<AttributeValue> values)
        {
            using (SqlConnection objConn = new SqlConnection(WebConfigurationManager.ConnectionStrings["eshopConnectionString"].ConnectionString))
                using (SqlBulkCopy sbc = new SqlBulkCopy(objConn))
                {
                    sbc.DestinationTableName = "attributeValue";
                    sbc.BatchSize = 2;
                    sbc.ColumnMappings.Add("attributeID", "attributeID");
                    sbc.ColumnMappings.Add("value", "value");
                    sbc.WriteToServer(Common.ConvertToDataTable(values));
                }
        }*/

        public eshopBE.Attribute GetAttribute(int attributeID)
        {
            return GetAttributesList(attributeID, string.Empty, -1)[0];
        }

        public eshopBE.Attribute GetAttribute(string name)
        {
            return GetAttributesList(-1, name, -1)[0];
        }

        public int SaveAttributeForCategory(int categoryID, int attributeID)
        {
            int status = 0;
            using (SqlConnection objConn = new SqlConnection(WebConfigurationManager.ConnectionStrings["eshopConnectionString"].ConnectionString))
            {
                using (SqlCommand objComm = new SqlCommand("INSERT INTO categoryAttribute (categoryID, attributeID) VALUES (@categoryID, @attributeID)", objConn))
                {
                    try
                    {
                        objConn.Open();
                        objComm.Parameters.Add("@categoryID", SqlDbType.Int).Value = categoryID;
                        objComm.Parameters.Add("@attributeID", SqlDbType.Int).Value = attributeID;

                        status = objComm.ExecuteNonQuery();
                    }
                    catch (SqlException ex)
                    {
                        if (ex.Message.Contains("Cannot insert duplicate key in object"))
                        {
                            throw new BLException("Atribut je već pridodat kategoriji", ex);
                        }
                        else
                        {
                            ErrorLog.LogError(ex);
                            throw new DLException("Error while saving attribute for category.", ex);
                        }
                    }
                }
            }
            return status;
        }

        public int DeleteAttributeForCategory(int categoryID, int attributeID)
        {
            int status = 0;
            using (SqlConnection objConn = new SqlConnection(WebConfigurationManager.ConnectionStrings["eshopConnectionString"].ConnectionString))
            {
                using (SqlCommand objComm = new SqlCommand("DELETE FROM categoryAttribute WHERE categoryID=@categoryID AND attributeID=@attributeID", objConn))
                {
                    try
                    {
                        objConn.Open();
                        objComm.Parameters.Add("@categoryID", SqlDbType.Int).Value = categoryID;
                        objComm.Parameters.Add("@attributeID", SqlDbType.Int).Value = attributeID;

                        status = objComm.ExecuteNonQuery();
                    }
                    catch (SqlException ex)
                    {
                        ErrorLog.LogError(ex);
                        throw new DLException("Error while deleting attribute for category", ex);
                    }
                }
            }
            return status;
        }

        public int SaveAttributeValue(AttributeValue attributeValue, bool isKimtec)
        {
            int attributeValueID = 0;
            using (SqlConnection objConn = new SqlConnection(WebConfigurationManager.ConnectionStrings["eshopConnectionString"].ConnectionString))
            {
                using (SqlCommand objComm = new SqlCommand("INSERT INTO attributeValue (attributeID, value, kimtecValue) VALUES (@attributeID, @value, @kimtecValue);SELECT SCOPE_IDENTITY()", objConn))
                {
                    try
                    {
                        objConn.Open();
                        objComm.Parameters.Add("@attributeID", SqlDbType.Int).Value = attributeValue.AttributeID;
                        objComm.Parameters.Add("@value", SqlDbType.NVarChar, 100).Value = attributeValue.Value;
                        objComm.Parameters.Add("@kimtecValue", SqlDbType.NVarChar, 200);
                        objComm.Parameters[2].Value = isKimtec ? (object)attributeValue.Value : DBNull.Value;

                        attributeValueID = int.Parse(objComm.ExecuteScalar().ToString());
                    }
                    catch (SqlException ex)
                    {
                        ErrorLog.LogError(ex);
                        throw new DLException("Error while saving attribute value.", ex);
                    }
                }
            }
            return attributeValueID;
        }

        public int DeleteAttributeValue(int attributeValueID)
        {
            int status = 0;
            using (SqlConnection objConn = new SqlConnection(WebConfigurationManager.ConnectionStrings["eshopConnectionString"].ConnectionString))
            {
                using (SqlCommand objComm = new SqlCommand("DELETE FROM attributeValue WHERE attributeValueID=@attributeValueID", objConn))
                {
                    try
                    {
                        objConn.Open();
                        objComm.Parameters.Add("@attributeValueID", SqlDbType.Int).Value = attributeValueID;

                        status = objComm.ExecuteNonQuery();
                    }
                    catch (SqlException ex)
                    {
                        ErrorLog.LogError(ex);
                        throw new DLException("Error while deleting attribute value.", ex);
                    }
                }
            }
            return status;
        }

        public int DeleteAttribute(int attributeID)
        {
            int status = 0;
            using (SqlConnection objConn = new SqlConnection(WebConfigurationManager.ConnectionStrings["eshopConnectionString"].ConnectionString))
            {
                using (SqlCommand objComm = new SqlCommand("DELETE FROM attribute WHERE attributeID=@attributeID", objConn))
                {
                    try
                    {
                        objConn.Open();
                        objComm.Parameters.Add("@attributeID", SqlDbType.Int).Value = attributeID;

                        status = objComm.ExecuteNonQuery();
                    }
                    catch(SqlException ex)
                    {
                        if (ex.Message.Contains("The DELETE statement conflicted with the REFERENCE constraint"))
                        {
                            throw new BLException("Nije moguće obrisati atribut. Postoje povezane kategorije ili proizvodi", ex);
                        }
                        else
                        {
                            ErrorLog.LogError(ex);
                            throw new DLException("Error while deleting attribute.", ex);
                        }
                    }
                }
            }
            return status;
        }

        public int SetFilter(int categoryID, int attributeID, bool filter)
        {
            int status = 0;
            using (SqlConnection objConn = new SqlConnection(WebConfigurationManager.ConnectionStrings["eshopConnectionString"].ConnectionString))
            {
                using (SqlCommand objComm = new SqlCommand("UPDATE categoryAttribute SET filter=@filter WHERE categoryID=@categoryID AND attributeID=@attributeID", objConn))
                {
                    objConn.Open();
                    objComm.Parameters.Add("@filter", SqlDbType.Bit).Value = filter;
                    objComm.Parameters.Add("@categoryID", SqlDbType.Int).Value = categoryID;
                    objComm.Parameters.Add("@attributeID", SqlDbType.Int).Value = attributeID;

                    status = objComm.ExecuteNonQuery();
                }
            }
            return status;
        }
        public int SetIsDescription(int categoryID, int attributeID, bool isDescription)
        {
            int status = 0;
            using (SqlConnection objConn = new SqlConnection(WebConfigurationManager.ConnectionStrings["eshopConnectionString"].ConnectionString))
            {
                using (SqlCommand objComm = new SqlCommand("UPDATE categoryAttribute SET isDescription=@isDescription WHERE categoryID=@categoryID AND attributeID=@attributeID", objConn))
                {
                    objConn.Open();
                    objComm.Parameters.Add("@isDescription", SqlDbType.Bit).Value = isDescription;
                    objComm.Parameters.Add("@categoryID", SqlDbType.Int).Value = categoryID;
                    objComm.Parameters.Add("@attributeID", SqlDbType.Int).Value = attributeID;

                    status = objComm.ExecuteNonQuery();
                }
            }
            return status;
        }

        public List<eshopBE.Attribute> GetAttributeListForFilter(string categoryUrl)
        {
            List<eshopBE.Attribute> attributes = null;
            eshopBE.Attribute attribute = null;

            using (SqlConnection objConn = new SqlConnection(WebConfigurationManager.ConnectionStrings["eshopConnectionString"].ConnectionString))
            {
                using (SqlCommand objComm = new SqlCommand("SELECT attribute.attributeID, attribute.name FROM attribute INNER JOIN categoryAttribute ON attribute.attributeID=categoryAttribute.attributeID INNER JOIN category ON categoryAttribute.categoryID=category.categoryID WHERE category.url=@categoryUrl AND filter=1 ORDER BY attribute.name", objConn))
                {
                    objConn.Open();
                    objComm.Parameters.Add("@categoryUrl", SqlDbType.NVarChar, 50).Value = categoryUrl;
                    using (SqlDataReader reader = objComm.ExecuteReader())
                    {
                        if (reader.HasRows)
                            attributes = new List<eshopBE.Attribute>();
                        while (reader.Read())
                        {
                            attribute = new eshopBE.Attribute();
                            attribute.AttributeID = reader.GetInt32(0);
                            attribute.Name = reader.GetString(1);
                            attribute.Filter = true;
                            attribute.Values = GetAttributeValuesForFilter(attribute.AttributeID);

                            attributes.Add(attribute);
                        }
                    }
                }
            }
            return attributes;
        }

        public int SaveAttributePositionForCategory(int attributeID, int categoryID, int position)
        {
            int status = 0;
            using (SqlConnection objConn = new SqlConnection(WebConfigurationManager.ConnectionStrings["eshopConnectionString"].ConnectionString))
            {
                using (SqlCommand objComm = new SqlCommand("saveAttributePositionForCategory", objConn))
                {
                    objConn.Open();
                    objComm.Parameters.Add("@attributeID", SqlDbType.Int).Value = attributeID;
                    objComm.Parameters.Add("@categoryID", SqlDbType.Int).Value = categoryID;
                    objComm.Parameters.Add("@position", SqlDbType.Int).Value = position;
                    objComm.CommandType = CommandType.StoredProcedure;

                    status = objComm.ExecuteNonQuery();
                }
            }
            return status;
        }

        public DataTable GetAttributesForMapping(int categoryID, int kimtecCategoryID)
        {
            DataTable attributes = null;
            using (SqlConnection objConn = new SqlConnection(WebConfigurationManager.ConnectionStrings["eshopConnectionString"].ConnectionString))
            {
                //using (SqlCommand objComm = new SqlCommand("SELECT attribute.attributeID, attribute.name, attributeKimtecAttribute.kimtecAttributeID FROM attribute LEFT JOIN categoryAttribute ON attribute.attributeID = categoryAttribute.attributeID LEFT JOIN attributeKimtecAttribute ON attribute.attributeID = attributeKimtecAttribute.attributeID	LEFT JOIN kimtecAttribute ON attributeKimtecAttribute.kimtecAttributeID = kimtecAttribute.kimtecAttributeID WHERE categoryAttribute.categoryID = @categoryID AND (kimtecAttribute.kimtecCategoryID = @kimtecCategoryID OR kimtecAttribute.kimtecCategoryID IS NULL)", objConn))
                using(SqlCommand objComm = new SqlCommand("attribute_getForMapping", objConn))
                {
                    objConn.Open();
                    objComm.Parameters.Add("@categoryID", SqlDbType.Int).Value = categoryID;
                    objComm.Parameters.Add("@kimtecCategoryID", SqlDbType.Int).Value = kimtecCategoryID;
                    objComm.CommandType = CommandType.StoredProcedure;
                    using (SqlDataReader reader = objComm.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            attributes = new DataTable();
                            attributes.Load(reader);
                        }
                    }
                }
            }
            return attributes;
        }

        public DataTable GetAttributesForKimtecCategory(int kimtecCategoryID)
        {
            DataTable attributes = null;
            using (SqlConnection objConn = new SqlConnection(WebConfigurationManager.ConnectionStrings["eshopConnectionString"].ConnectionString))
            {
                using (SqlCommand objComm = new SqlCommand("attributes_getForKimtecCategory", objConn))
                {
                    objConn.Open();
                    objComm.Parameters.Add("@kimtecCategoryID", SqlDbType.Int).Value = kimtecCategoryID;
                    objComm.CommandType = CommandType.StoredProcedure;
                    using (SqlDataReader reader = objComm.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            attributes = new DataTable();
                            attributes.Load(reader);
                        }
                    }
                }
            }
            return attributes;
        }

        public DataTable GetAttributeValuesForKimtec(int attributeID)
        {
            DataTable attributeValues = null;
            using (SqlConnection objConn = new SqlConnection(WebConfigurationManager.ConnectionStrings["eshopConnectionString"].ConnectionString))
            {
                using (SqlCommand objComm = new SqlCommand("attributeValue_getForKimtec", objConn))
                {
                    objConn.Open();
                    objComm.Parameters.Add("@attributeID", SqlDbType.Int).Value = @attributeID;
                    objComm.CommandType = CommandType.StoredProcedure;
                    using (SqlDataReader reader = objComm.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            attributeValues = new DataTable();
                            attributeValues.Load(reader);
                        }
                    }
                }
            }
            return attributeValues;
        }

        public int SaveAttributeValueSort(int attributeValueID, int sort)
        {
            int status = 0;
            using (SqlConnection objConn = new SqlConnection(WebConfigurationManager.ConnectionStrings["eshopConnectionString"].ConnectionString))
            {
                using (SqlCommand objComm = new SqlCommand("attributeValue_saveSort", objConn))
                {
                    objConn.Open();
                    objComm.CommandType = CommandType.StoredProcedure;
                    objComm.Parameters.Add("@attributeValueID", attributeValueID);
                    objComm.Parameters.Add("@sort", sort);

                    status = objComm.ExecuteNonQuery();
                }
            }
            return status;
        }
    }
}
