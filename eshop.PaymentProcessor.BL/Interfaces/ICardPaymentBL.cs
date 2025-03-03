using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eshop.PaymentProcessor.BL.Interfaces
{
    public interface ICardPaymentBL
    {
        string GetPostRedirectedTemplate(double amountValue, string orderID, string paymentFormTemplatePath);
        string CalculateHash(string clientID, string orderID, string amount, string okUrl, string failUrl, string tranType, string instalment, string rnd, string currency, string storeKey);
    }
}
