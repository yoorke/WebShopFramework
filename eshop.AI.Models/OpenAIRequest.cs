using System.Collections.Generic;

namespace eshop.AI.Models
{
    public class OpenAIRequest
    {
        public string model { get; set; }
        public float temperature { get; set; }
        public List<OpenAIRequestMessage> messages { get; set; }
    }
}
