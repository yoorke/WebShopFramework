using eshop.PaymentProcessor.BE;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eshop.PaymentProcessor.DL.Interfaces
{
    public interface IPaymentResponseDL
    {
        void SavePaymentResponse(PaymentResponse paymentResponse);
    }
}
