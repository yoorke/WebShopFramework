using System.Threading.Tasks;

namespace eshop.AI.BL.Interfaces
{
    public interface IAIHandler
    {
        Task<string> SendRequestAsync(string model, float temperature, string systemMessage, string userMessage);
    }
}
