using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eshop.PaymentProcessor.BE
{
    public class CardPaymentParameters
    {
        public string MerchantId { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string PaymentGateUrl { get; set; }
        public string OkUrl { get; set; }
        public string FailUrl { get; set; }
        public string StoreKey { get; set; }
        public string StoreType { get; set; }
        public string Currency { get; set; }
        public string Language { get; set; }
        public string HashAlgorithmType { get; set; }        
        public string Instalment { get; set; }
        public string TranType { get; set; }
        public string MandatoryFields { get; set; }
        public string PaymentParameters { get; set; }
    }
}
