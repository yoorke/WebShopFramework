using eshop.PaymentProcessor.BE;
using eshop.PaymentProcessor.BL.Interfaces;
using eshop.PaymentProcessor.DL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eshop.PaymentProcessor.BL
{
    public class PaymentResponseBL : IPaymentResponseBL
    {
        public void SavePaymentResponse(PaymentResponse paymentResponse)
        {
            new PaymentResponseDL().SavePaymentResponse(paymentResponse);
        }
    }
}
