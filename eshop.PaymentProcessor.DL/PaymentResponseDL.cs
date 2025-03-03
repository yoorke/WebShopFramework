using eshop.PaymentProcessor.BE;
using eshop.PaymentProcessor.DL.Interfaces;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace eshop.PaymentProcessor.DL
{
    public class PaymentResponseDL : IPaymentResponseDL
    {
        public void SavePaymentResponse(PaymentResponse paymentResponse)
        {
            using (SqlConnection objConn = new SqlConnection(ConfigurationManager.ConnectionStrings["eshopConnectionString"].ConnectionString))
            {
                using (SqlCommand objComm = new SqlCommand("paymentResponse_insert", objConn))
                {
                    objConn.Open();
                    objComm.CommandType = CommandType.StoredProcedure;

                    objComm.Parameters.Add("@returnOid", SqlDbType.NVarChar, 100).Value = paymentResponse.ReturnOid ?? (object)DBNull.Value;
                    objComm.Parameters.Add("@tranId", SqlDbType.NVarChar, 100).Value = paymentResponse.TranId ?? (object)DBNull.Value;
                    objComm.Parameters.Add("@hashAlgorithm", SqlDbType.NVarChar, 100).Value = paymentResponse.HashAlgorithm ?? (object)DBNull.Value;
                    objComm.Parameters.Add("@merchantId", SqlDbType.NVarChar, 100).Value = paymentResponse.MerchantID ?? (object)DBNull.Value;
                    objComm.Parameters.Add("@acqbin", SqlDbType.NVarChar, 100).Value = paymentResponse.Acqbin ?? (object)DBNull.Value;
                    objComm.Parameters.Add("@cardExpYear", SqlDbType.NVarChar, 100).Value = paymentResponse.Ecom_Payment_Card_ExpDate_Year ?? (object)DBNull.Value;
                    objComm.Parameters.Add("@cardExpMonth", SqlDbType.NVarChar, 100).Value = paymentResponse.Ecom_Payment_Card_ExpDate_Month ?? (object)DBNull.Value;
                    objComm.Parameters.Add("@extraCardBrand", SqlDbType.NVarChar, 100).Value = paymentResponse.ExtraCardBrand ?? (object)DBNull.Value;
                    objComm.Parameters.Add("@acqstan", SqlDbType.NVarChar, 100).Value = paymentResponse.AcqStan ?? (object)DBNull.Value;
                    objComm.Parameters.Add("@clientIP", SqlDbType.NVarChar, 100).Value = paymentResponse.clientIP ?? (object)DBNull.Value;
                    objComm.Parameters.Add("@md", SqlDbType.NVarChar, 100).Value = paymentResponse.Md ?? (object)DBNull.Value;
                    objComm.Parameters.Add("@procReturnCode", SqlDbType.NVarChar, 100).Value = paymentResponse.ProcReturnCode ?? (object)DBNull.Value;
                    objComm.Parameters.Add("@transId", SqlDbType.NVarChar, 100).Value = paymentResponse.TransId ?? (object)DBNull.Value;
                    objComm.Parameters.Add("@response", SqlDbType.NVarChar, 100).Value = paymentResponse.Response ?? (object)DBNull.Value;
                    objComm.Parameters.Add("@settleId", SqlDbType.NVarChar, 100).Value = paymentResponse.SettleId ?? (object)DBNull.Value;
                    objComm.Parameters.Add("@mdERrorMsg", SqlDbType.NVarChar, 100).Value = paymentResponse.MdErrorMsg ?? (object)DBNull.Value;
                    objComm.Parameters.Add("@errMsg", SqlDbType.NVarChar, 1000).Value = paymentResponse.ErrMsg ?? (object)DBNull.Value;
                    objComm.Parameters.Add("@hostRefNum", SqlDbType.NVarChar, 100).Value = paymentResponse.HostRefNum ?? (object)DBNull.Value;
                    objComm.Parameters.Add("@authCode", SqlDbType.NVarChar, 100).Value = paymentResponse.AuthCode ?? (object)DBNull.Value;
                    objComm.Parameters.Add("@xid", SqlDbType.NVarChar, 100).Value = paymentResponse.Xid ?? (object)DBNull.Value;
                    objComm.Parameters.Add("@oid", SqlDbType.NVarChar, 100).Value = paymentResponse.Oid ?? (object)DBNull.Value;
                    objComm.Parameters.Add("@hash", SqlDbType.NVarChar, 2000).Value = paymentResponse.Hash ?? (object)DBNull.Value;
                    objComm.Parameters.Add("@amount", SqlDbType.NVarChar, 100).Value = paymentResponse.Amount ?? (object)DBNull.Value;
                    objComm.Parameters.Add("@hashParams", SqlDbType.NVarChar, 1000).Value = paymentResponse.HashParams ?? (object)DBNull.Value;
                    objComm.Parameters.Add("@hashParamsVal", SqlDbType.NVarChar, 1000).Value = paymentResponse.HashParamsVal ?? (object)DBNull.Value;
                    objComm.Parameters.Add("@date", SqlDbType.DateTime).Value = paymentResponse.Date;
                    objComm.Parameters.Add("@orderId", SqlDbType.Int).Value = paymentResponse.OrderID;

                    objComm.ExecuteNonQuery();
                }
            }
        }
    }
}
