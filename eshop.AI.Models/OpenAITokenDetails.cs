namespace eshop.AI.Models
{
    public class OpenAITokenDetails
    {
        public int CachedTokens { get; set; }
        public int AudioTokens { get; set; }
        public int ReasoningTokens { get; set; }
        public int AcceptedPredictionTokens { get; set; }
        public int RejectedPredictionTokens { get; set; }
    }
}
