namespace eshop.AI.Models
{
    public class OpenAIResponseMessage
    {
        public string Role { get; set; }
        public string Content { get; set; }
        public object Refusal { get; set; }
    }
}
