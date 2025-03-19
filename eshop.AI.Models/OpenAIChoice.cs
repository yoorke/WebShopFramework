namespace eshop.AI.Models
{
    public class OpenAIChoice
    {
        public int Index { get; set; }
        public OpenAIResponseMessage Message { get; set; }
        public object Logprobs { get; set; }
        public string FinishReason { get; set; }
    }
}
