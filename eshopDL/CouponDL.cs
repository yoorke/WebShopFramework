using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eshopBE;
using eshopUtilities;
using System.Data;
using System.Data.SqlClient;
using System.Web.Configuration;

namespace eshopDL
{
    public class CouponDL
    {
        public Coupon GetCoupon(string code)
        {
            Coupon coupon = null;
            using (SqlConnection objConn = new SqlConnection(WebConfigurationManager.ConnectionStrings["eshopConnectionString"].ConnectionString))
            {
                using (SqlCommand objComm = new SqlCommand("getCouponByCode", objConn))
                {
                    objConn.Open();
                    objComm.Parameters.Add("@code", SqlDbType.NChar, 10).Value = code;
                    objComm.CommandType = CommandType.StoredProcedure;
                    using (SqlDataReader reader = objComm.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            coupon = new Coupon(reader.GetInt32(0), reader.GetString(1), reader.GetDouble(2), code, reader.GetDateTime(4), reader.GetDateTime(5), new CouponType(reader.GetInt32(6), reader.GetString(7)), null);
                            coupon.Objects = getCouponObjects(coupon.CouponID);
                        }
                    }
                }
            }
            return coupon;
        }

        public int Save(Coupon coupon)
        {
            int status = 0;
            using (SqlConnection objConn = new SqlConnection(WebConfigurationManager.ConnectionStrings["eshopConnectionString"].ConnectionString))
            {
                using (SqlCommand objComm = new SqlCommand("coupon_insert", objConn))
                {
                    objConn.Open();
                    objComm.CommandType = CommandType.StoredProcedure;
                    objComm.Parameters.Add("@name", SqlDbType.NVarChar, 50).Value = coupon.Name;
                    objComm.Parameters.Add("@code", SqlDbType.NVarChar, 50).Value = coupon.Code;
                    objComm.Parameters.Add("@dateFrom", SqlDbType.Date).Value = coupon.DateFrom;
                    objComm.Parameters.Add("@dateTo", SqlDbType.Date).Value = coupon.DateTo;
                    objComm.Parameters.Add("@couponTypeID", SqlDbType.Int).Value = coupon.CouponType.CouponTypeID;
                    objComm.Parameters.Add("@discount", SqlDbType.Float).Value = coupon.Discount;

                    coupon.CouponID = int.Parse(objComm.ExecuteScalar().ToString());

                    saveCouponObjects(coupon);
                }
            }
            return coupon.CouponID;
        }

        public int Update(Coupon coupon)
        {
            int status = 0;
            using (SqlConnection objConn = new SqlConnection(WebConfigurationManager.ConnectionStrings["eshopConnectionString"].ConnectionString))
            {
                using (SqlCommand objComm = new SqlCommand("coupon_update", objConn))
                {
                    objConn.Open();
                    objComm.CommandType = CommandType.StoredProcedure;
                    objComm.Parameters.Add("@name", SqlDbType.NVarChar, 50).Value = coupon.Name;
                    objComm.Parameters.Add("@code", SqlDbType.NVarChar, 50).Value = coupon.Code;
                    objComm.Parameters.Add("@dateFrom", SqlDbType.Date).Value = coupon.DateFrom;
                    objComm.Parameters.Add("@dateTo", SqlDbType.Date).Value = coupon.DateTo;
                    objComm.Parameters.Add("@couponTypeID", SqlDbType.Int).Value = coupon.CouponType.CouponTypeID;
                    objComm.Parameters.Add("@discount", SqlDbType.Float).Value = coupon.Discount;
                    objComm.Parameters.Add("@couponID", SqlDbType.Int).Value = coupon.CouponID;

                    status = objComm.ExecuteNonQuery();

                    saveCouponObjects(coupon);
                }
            }
            return coupon.CouponID;
        }

        private void saveCouponObjects(Coupon coupon)
        {
            deleteCouponObjects(coupon.CouponID);
            using (SqlConnection objConn = new SqlConnection(WebConfigurationManager.ConnectionStrings["eshopConnectionString"].ConnectionString))
            {
                using (SqlCommand objComm = new SqlCommand("couponObject_insert", objConn))
                {
                    objConn.Open();
                    objComm.CommandType = CommandType.StoredProcedure;
                    foreach(CouponObject couponObject in coupon.Objects)
                    {
                        objComm.Parameters.Clear();
                        objComm.Parameters.Add("@couponID", SqlDbType.Int).Value = coupon.CouponID;
                        objComm.Parameters.Add("@objectTypeID", SqlDbType.Int).Value = couponObject.ObjectTypeID;
                        objComm.Parameters.Add("@objectID", SqlDbType.Int).Value = couponObject.ObjectID;
                        objComm.Parameters.Add("@name", SqlDbType.NVarChar, 50).Value = couponObject.Name;

                        objComm.ExecuteNonQuery();
                    }
                }
            }
        }

        private void deleteCouponObjects(int couponID)
        {
            using (SqlConnection objConn = new SqlConnection(WebConfigurationManager.ConnectionStrings["eshopConnectionString"].ConnectionString))
            {
                using (SqlCommand objComm = new SqlCommand("couponObject_delete", objConn))
                {
                    objConn.Open();
                    objComm.CommandType = CommandType.StoredProcedure;
                    objComm.Parameters.Add("@couponID", SqlDbType.Int).Value = couponID;

                    objComm.ExecuteNonQuery();
                }
            }
        }

        private List<CouponObject> getCouponObjects(int couponID)
        {
            List<CouponObject> objects = new List<CouponObject>();
            using (SqlConnection objConn = new SqlConnection(WebConfigurationManager.ConnectionStrings["eshopConnectionString"].ConnectionString))
            {
                using (SqlCommand objComm = new SqlCommand("couponObject_get", objConn))
                {
                    objConn.Open();
                    objComm.CommandType = CommandType.StoredProcedure;
                    objComm.Parameters.Add("@couponID", SqlDbType.Int).Value = couponID;
                    using (SqlDataReader reader = objComm.ExecuteReader())
                    {
                        while (reader.Read())
                            objects.Add(new CouponObject(couponID, reader.GetInt32(2), reader.GetInt32(1), reader.GetString(3)));
                    }
                }
            }
            return objects;
        }

        public List<Coupon> GetCoupons()
        {
            List<Coupon> coupons = new List<Coupon>();
            using (SqlConnection objConn = new SqlConnection(WebConfigurationManager.ConnectionStrings["eshopConnectionString"].ConnectionString))
            {
                using (SqlCommand objComm = new SqlCommand("coupon_get", objConn))
                {
                    objConn.Open();
                    objComm.CommandType = CommandType.StoredProcedure;
                    using (SqlDataReader reader = objComm.ExecuteReader())
                    {
                        while (reader.Read())
                            coupons.Add(new Coupon(reader.GetInt32(0), reader.GetString(1), reader.GetDouble(7), reader.GetString(2), reader.GetDateTime(3), reader.GetDateTime(4), new CouponType(reader.GetInt32(5), reader.GetString(6)), null));
                    }
                }
            }
            return coupons;
        }

        public Coupon GetCoupon(int couponID)
        {
            Coupon coupon = null;
            using (SqlConnection objConn = new SqlConnection(WebConfigurationManager.ConnectionStrings["eshopConnectionString"].ConnectionString))
            {
                using (SqlCommand objComm = new SqlCommand("coupon_select", objConn))
                {
                    objConn.Open();
                    objComm.CommandType = CommandType.StoredProcedure;
                    objComm.Parameters.Add("@couponID", SqlDbType.Int).Value = couponID;
                    using (SqlDataReader reader = objComm.ExecuteReader())
                    {
                        while (reader.Read())
                        { 
                            coupon = new Coupon(reader.GetInt32(0), reader.GetString(1), reader.GetDouble(7), reader.GetString(2), reader.GetDateTime(3), reader.GetDateTime(4), new CouponType(reader.GetInt32(5), reader.GetString(6)), null);
                            coupon.Objects = getCouponObjects(coupon.CouponID);
                        }
                    }
                }
            }
            return coupon;
        }

        public List<CouponType> GetCouponTypes()
        {
            List<CouponType> couponTypes = new List<CouponType>();
            using (SqlConnection objConn = new SqlConnection(WebConfigurationManager.ConnectionStrings["eshopConnectionString"].ConnectionString))
            {
                using (SqlCommand objComm = new SqlCommand("couponType_get", objConn))
                {
                    objConn.Open();
                    objComm.CommandType = CommandType.StoredProcedure;
                    using (SqlDataReader reader = objComm.ExecuteReader())
                    {
                        while (reader.Read())
                            couponTypes.Add(new CouponType(reader.GetInt32(0), reader.GetString(1)));
                    }
                }
            }
            return couponTypes;
        }

        public int DeleteCoupon(int couponID)
        {
            int status = 0;
            using (SqlConnection objConn = new SqlConnection(WebConfigurationManager.ConnectionStrings["eshopConnectionString"].ConnectionString))
            {
                using (SqlCommand objComm = new SqlCommand("coupon_delete", objConn))
                {
                    objConn.Open();
                    objComm.CommandType = CommandType.StoredProcedure;
                    objComm.Parameters.Add("@couponID", SqlDbType.Int).Value = couponID;
                    status = objComm.ExecuteNonQuery();
                }
            }
            return status;
        }
    }
}
