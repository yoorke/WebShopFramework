using eshop.PaymentProcessor.BE;
using System.Threading.Tasks;

namespace eshop.PaymentProcessor.BL.Interfaces
{
    public interface IIPSGenerator
    {
        Task<bool> GenerateIPSQRCodeAsync(IPSQRCodeRequest request, string imagePath);
    }
}
