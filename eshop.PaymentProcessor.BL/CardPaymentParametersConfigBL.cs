using eshop.PaymentProcessor.BE;
using eshop.PaymentProcessor.BL.Interfaces;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eshop.PaymentProcessor.BL
{
    public class CardPaymentParametersConfigBL : ICardPaymentParametersBL
    {
        public CardPaymentParameters LoadParameters()
        {
            CardPaymentParameters parameters = new CardPaymentParameters();

            parameters.MerchantId = ConfigurationManager.AppSettings["PPMerchantID"];
            parameters.Username = ConfigurationManager.AppSettings["PPUsername"];
            parameters.Password = ConfigurationManager.AppSettings["PPPassword"];
            parameters.PaymentGateUrl = ConfigurationManager.AppSettings["PP3dGateUrl"];
            parameters.OkUrl = ConfigurationManager.AppSettings["PPOkUrl"];
            parameters.FailUrl = ConfigurationManager.AppSettings["PPFailUrl"];
            parameters.StoreKey = ConfigurationManager.AppSettings["PPStoreKey"];
            parameters.StoreType = ConfigurationManager.AppSettings["PPStoreType"];
            parameters.Currency = ConfigurationManager.AppSettings["PPCurrency"];
            parameters.Language = ConfigurationManager.AppSettings["PPLanguage"];
            parameters.HashAlgorithmType = ConfigurationManager.AppSettings["PPHashAlgorithmType"];
            parameters.MandatoryFields = ConfigurationManager.AppSettings["PPMandatoryFields"];
            parameters.PaymentParameters = ConfigurationManager.AppSettings["PPPaymentParameters"];
            parameters.TranType = ConfigurationManager.AppSettings["PPTranType"];
            parameters.Instalment = ConfigurationManager.AppSettings["PPInstalment"];

            return parameters;
        }
    }
}
