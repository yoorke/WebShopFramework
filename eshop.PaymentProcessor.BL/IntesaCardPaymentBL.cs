using eshop.PaymentProcessor.BE;
using eshop.PaymentProcessor.BL.Interfaces;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace eshop.PaymentProcessor.BL
{
    public class IntesaCardPaymentBL : ICardPaymentBL
    {
        public string GetPostRedirectedTemplate(double amountValue, string orderID, string paymentFormTemplatePath)
        {
            string sb = string.Empty;
            CardPaymentParameters parameters = new CardPaymentParametersConfigBL().LoadParameters();

            using (TextReader tr = new StreamReader(paymentFormTemplatePath))
            {
                //string amount = string.Format(new CultureInfo("sr-Latn"), "{0:0.00}", amountValue).ToString();
                string amount = string.Format("{0:0.00}", amountValue);
                amount = amount.Replace(',', '.');

                string rnd = DateTime.Now.ToString("ddMMyyyyHHmmss");

                sb = tr.ReadToEnd();
                sb = sb.Replace("{action}", parameters.PaymentGateUrl);
                sb = sb.Replace("{clientID}", parameters.MerchantId);
                sb = sb.Replace("{amount}", amount);
                sb = sb.Replace("{orderID}", orderID);
                sb = sb.Replace("{okUrl}", parameters.OkUrl);
                sb = sb.Replace("{failUrl}", parameters.FailUrl);
                sb = sb.Replace("{tranType}", parameters.TranType);
                sb = sb.Replace("{instalment}", parameters.Instalment);
                sb = sb.Replace("{currency}", parameters.Currency);
                sb = sb.Replace("{rnd}", rnd);
                sb = sb.Replace("{hash}", CalculateHash(parameters.MerchantId, orderID, amount, parameters.OkUrl, parameters.FailUrl, parameters.TranType, parameters.Instalment, rnd, parameters.Currency, parameters.StoreKey));
                sb = sb.Replace("{storeType}", parameters.StoreType);
                sb = sb.Replace("{language}", parameters.Language);
                sb = sb.Replace("{hashAlgorithm}", parameters.HashAlgorithmType);
                sb = sb.Replace("{shopUrl}", ConfigurationManager.AppSettings["webShopUrl"]);
            }

            return sb;
        }

        public string CalculateHash(string clientID, string orderID, string amount, string okUrl, string failUrl, string tranType, string instalment, string rnd, string currency, string storeKey)
        {
            string text = clientID + "|" + orderID + "|" + amount + "|" + okUrl + "|" + failUrl + "|" + tranType + "||" + rnd + "||||" + currency + "|" + storeKey;
            SHA512 sha = new SHA512CryptoServiceProvider();
            byte[] hashBytes = Encoding.GetEncoding("utf-8").GetBytes(text);
            byte[] inputBytes = sha.ComputeHash(hashBytes);
            return Convert.ToBase64String(inputBytes);
        }
    }
}
