using System.Collections.Generic;

namespace eshop.AI.Models
{
    public class OpenAIChatCompletionResponse
    {
        public string Id { get; set; }
        public string Object { get; set; }
        public long Created { get; set; }
        public string Model { get; set; }
        public List<OpenAIChoice> Choices { get; set; }
        public OpenAIUsage Usage { get; set; }
        public string ServiceTier { get; set; }
        public string SystemFingerprint { get; set; }
    }
}
