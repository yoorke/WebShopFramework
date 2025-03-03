using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace eshop.PaymentProcessor.BE
{
    public class PaymentResponse
    {
        public string ReturnOid { get; set; }
        public string TranId { get; set; }
        public string HashAlgorithm { get; set; }
        public string MerchantID { get; set; }
        public string Acqbin { get; set; }
        public string Ecom_Payment_Card_ExpDate_Year { get; set; }
        public string Ecom_Payment_Card_ExpDate_Month { get; set; }
        public string ExtraCardBrand { get; set; }
        public string AcqStan { get; set; }
        public string clientIP { get; set; }
        public string Md { get; set; }
        public string ProcReturnCode { get; set; }
        public string TransId { get; set; }
        public string Response { get; set; }
        public string SettleId { get; set; }
        public string MdErrorMsg { get; set; }
        public string ErrMsg { get; set; }
        public string HostRefNum { get; set; }
        public string AuthCode { get; set; }
        public string Xid { get; set; }
        public string Oid { get; set; }
        public string Hash { get; set; }
        public string Amount { get; set; }
        public string HashParams { get; set; }
        public string HashParamsVal { get; set; }
        public DateTime Date { get; set; }
        public int OrderID { get; set; }
    }
}