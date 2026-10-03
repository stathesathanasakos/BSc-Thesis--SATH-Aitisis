namespace SATH.Aitisis.Infrastructure.Ai.Groq
{
    public sealed class GroqAiOptions
    {
        public string ApiKey { get; init; } = string.Empty;
        //public string Model { get; init; } = "llama-3.3-70b-versatile";
        public string Model { get; init; } =  "openai/gpt-oss-120b";
        public string BaseUrl { get; init; } = "https://api.groq.com/openai/v1/chat/completions";
    }
}