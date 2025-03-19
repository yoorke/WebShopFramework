namespace eshop.AI.Models
{
    public class OpenAIUsage
    {
        public int PromptTokens { get; set; }
        public int CompletionTokens { get; set; }
        public int TotalTokens { get; set; }
        public OpenAITokenDetails PromptTokensDetails { get; set; }
        public OpenAITokenDetails CompletionTokensDetails { get; set; }
    }
}
