using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eshopBE;
using System.Data;
using System.Data.SqlClient;
using System.Web.Configuration;
using eshopUtilities;

namespace eshopDL
{
    public class SliderDL
    {
        public int SaveSlider(Slider slider)
        {
            int status = 0;
            try
            {
                //SqlTransaction transaction = null;
                using (SqlConnection objConn = new SqlConnection(WebConfigurationManager.ConnectionStrings["eshopConnectionString"].ConnectionString))
                {
                    objConn.Open();
                    using (SqlCommand objComm = objConn.CreateCommand())
                    {
                        objComm.Connection = objConn;
                        objComm.CommandText = "saveSlider";
                        objComm.CommandType = CommandType.StoredProcedure;

                        //using (SqlTransaction transaction = objConn.BeginTransaction())
                        //{
                            //objComm.Transaction = transaction;

                            objComm.Parameters.Add("@name", SqlDbType.NVarChar, 50).Value = slider.Name;
                            objComm.Parameters.Add("@insertDate", SqlDbType.DateTime).Value = slider.InsertDate;
                            objComm.Parameters.Add("@active", SqlDbType.Bit).Value = slider.Active;

                            SqlParameter sliderID = new SqlParameter("@sliderID", SqlDbType.Int);
                            sliderID.Direction = ParameterDirection.Output;
                            objComm.Parameters.Add(sliderID);

                            status = objComm.ExecuteNonQuery();

                            slider.SliderID = int.Parse(objComm.Parameters["@sliderID"].Value.ToString());

                            if (slider.SliderID > 0)
                            {
                                if (slider.Items != null)
                                    foreach (SliderItem item in slider.Items)
                                    {
                                        item.SliderID = slider.SliderID;
                                        SaveSliderItem(item);
                                    }
                            }

                            //if (status > 0)
                            //{
                                //objComm.CommandText = "saveSliderItem";
                                //for (int i = 0; i < slider.Items.Count; i++)
                                //{
                                    //objComm.Parameters.Clear();
                                    //objComm.Parameters.Add("@sliderID", SqlDbType.Int).Value = sliderID;
                                    //objComm.Parameters.Add("@imageUrl", SqlDbType.NVarChar, 50).Value = slider.Items[i].ImageUrl;

                                    //status = objComm.ExecuteNonQuery();
                                //}

                            //}
                            //transaction.Commit();
                        //}
                    }
                }

            }
            catch (SqlException ex)
            {

            }
            finally
            {

            }
            return status;
        }

        public int UpdateSlider(Slider slider)
        {
            int status = 0;
            using (SqlConnection objConn = new SqlConnection(WebConfigurationManager.ConnectionStrings["eshopConnectionString"].ConnectionString))
            {
                using (SqlCommand objComm = new SqlCommand("updateSlider", objConn))
                {
                    objConn.Open();
                    objComm.CommandType = CommandType.StoredProcedure;
                    objComm.Parameters.Add("@name", SqlDbType.NVarChar, 50).Value = slider.Name;
                    objComm.Parameters.Add("@sliderID", SqlDbType.Int).Value = slider.SliderID;
                    objComm.Parameters.Add("@updateDate", SqlDbType.DateTime).Value = slider.UpdateDate;
                    objComm.Parameters.Add("@active", SqlDbType.Bit).Value = slider.Active;

                    status = objComm.ExecuteNonQuery();

                    foreach (SliderItem item in slider.Items)
                    {
                        item.SliderID = slider.SliderID;
                        SaveSliderItem(item);
                    }
                }
            }
            return 0;
        }

        public Slider GetSlider(int sliderID)
        {
            Slider slider = null;
            using (SqlConnection objConn = new SqlConnection(WebConfigurationManager.ConnectionStrings["eshopConnectionString"].ConnectionString))
            {
                using (SqlCommand objComm = new SqlCommand("getSlider", objConn))
                {
                    objConn.Open();
                    objComm.CommandType = CommandType.StoredProcedure;
                    objComm.Parameters.Add("@sliderID", SqlDbType.Int).Value = sliderID;
                    using (SqlDataReader reader = objComm.ExecuteReader())
                    {
                        if(reader.HasRows)
                            while (reader.Read())
                            {
                                slider = new Slider(sliderID, reader.GetString(0), Common.ConvertToLocalTime(reader.GetDateTime(2)), Common.ConvertToLocalTime(reader.GetDateTime(3)), reader.GetBoolean(1));
                                slider.Items = getSliderItems(slider.SliderID);
                            }
                    }
                }
            }
            return slider;
        }

        public DataTable GetSliders()
        {
            DataTable sliders = null;
            using (SqlConnection objConn = new SqlConnection(WebConfigurationManager.ConnectionStrings["eshopConnectionString"].ConnectionString))
            {
                using (SqlCommand objComm = new SqlCommand("getSliders", objConn))
                {
                    objConn.Open();
                    objComm.CommandType = CommandType.StoredProcedure;
                    using (SqlDataReader reader = objComm.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            sliders = new DataTable();
                            sliders.Load(reader);
                        }
                    }
                }
            }
            return Common.ConvertToLocalTime(ref sliders);
        }

        public List<SliderItem> getSliderItems(int sliderID)
        {
            List<SliderItem> items = null;
            using (SqlConnection objConn = new SqlConnection(WebConfigurationManager.ConnectionStrings["eshopConnectionString"].ConnectionString))
            {
                using (SqlCommand objComm = new SqlCommand("getSliderItems", objConn))
                {
                    objConn.Open();
                    objComm.CommandType = CommandType.StoredProcedure;
                    objComm.Parameters.Add("@sliderID", SqlDbType.Int).Value = sliderID;
                    using (SqlDataReader reader = objComm.ExecuteReader())
                    {
                        if (reader.HasRows)
                            items = new List<SliderItem>();
                        while (reader.Read())
                            items.Add(new SliderItem(reader.GetInt32(0), sliderID, reader.GetString(1), reader.GetInt32(2), Convert.IsDBNull(reader[3]) == false ? reader.GetString(3) : string.Empty));
                    }
                }
            }
            return items;
        }

        public int SaveSliderItem(SliderItem item)
        {
            int status = 0;
            using (SqlConnection objConn = new SqlConnection(WebConfigurationManager.ConnectionStrings["eshopConnectionString"].ConnectionString))
            {
                using (SqlCommand objComm = new SqlCommand("saveSliderItem", objConn))
                {
                    objConn.Open();
                    objComm.CommandType = CommandType.StoredProcedure;
                    objComm.Parameters.Add("@sliderID", SqlDbType.Int).Value = item.SliderID;
                    objComm.Parameters.Add("@imageUrl", SqlDbType.NVarChar, 50).Value = item.ImageUrl;
                    objComm.Parameters.Add("@sortIndex", SqlDbType.Int).Value = item.SortIndex;
                    objComm.Parameters.Add("@url", SqlDbType.NVarChar, 100).Value = item.Url;

                    status = objComm.ExecuteNonQuery();
                }
            }
            return status;
        }

        public int DeleteSliderItem(int sliderItemID)
        {
            int status = 0;
            using (SqlConnection objConn = new SqlConnection(WebConfigurationManager.ConnectionStrings["eshopConnectionString"].ConnectionString))
            {
                using(SqlCommand objComm=new SqlCommand("deleteSliderItem",objConn))
                {
                    objConn.Open();
                    objComm.CommandType=CommandType.StoredProcedure;
                    objComm.Parameters.Add("@sliderItemID",SqlDbType.Int).Value=sliderItemID;

                    status=objComm.ExecuteNonQuery();
                }
            }
            return status;
        }

        public int DeleteSlider(int sliderID)
        {
            int status = 0;
            using (SqlConnection objConn = new SqlConnection(WebConfigurationManager.ConnectionStrings["eshopConnectionString"].ConnectionString))
            {
                using (SqlCommand objComm = new SqlCommand("deleteSlider", objConn))
                {
                    objConn.Open();
                    objComm.CommandType = CommandType.StoredProcedure;
                    objComm.Parameters.Add("@sliderID", SqlDbType.Int).Value = sliderID;

                    status = objComm.ExecuteNonQuery();
                }
            }
            return status;
        }

        public void ReorderSliderItem(int sliderItemID, int index, int sliderID)
        {
            using (SqlConnection objConn = new SqlConnection(WebConfigurationManager.ConnectionStrings["eshopConnectionString"].ConnectionString))
            {
                using (SqlCommand objComm = new SqlCommand("reorderSliderItems", objConn))
                {
                    objConn.Open();
                    objComm.CommandType = CommandType.StoredProcedure;
                    objComm.Parameters.Add("@sliderItemID", SqlDbType.Int).Value = sliderItemID;
                    objComm.Parameters.Add("@index", SqlDbType.Int).Value = index;
                    objComm.Parameters.Add("@sliderID", SqlDbType.Int).Value = sliderID;

                    objComm.ExecuteNonQuery();
                }
            }
        }
    }
}
