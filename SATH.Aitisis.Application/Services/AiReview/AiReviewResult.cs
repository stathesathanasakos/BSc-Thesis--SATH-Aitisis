namespace SATH.Aitisis.App.Services.AiReview
{
    // this is the structured output 
    public sealed class AiReviewResult
    {
        public bool Succeeded { get; init; }     // Indicates whether the AI review generation was successful or not 

        public string Summary { get; init; } = string.Empty;

        public IReadOnlyCollection<string> KeyInformation { get; init; } = Array.Empty<string>();

        public IReadOnlyCollection<string> PdfReviewFocus { get; init; } = Array.Empty<string>();

        public string Disclaimer { get; init; } = string.Empty;

        public string? ErrorMessage { get; init; }

        public static AiReviewResult Success(string summary, IReadOnlyCollection<string> keyInformation, IReadOnlyCollection<string> pdfReviewFocus, string disclaimer)
        {
            return new AiReviewResult
            {
                Succeeded = true,
                Summary = summary,
                KeyInformation = keyInformation,
                PdfReviewFocus = pdfReviewFocus,
                Disclaimer = disclaimer
            };
        }

        public static AiReviewResult Failure(string errorMessage)
        {
            return new AiReviewResult
            {
                Succeeded = false,
                ErrorMessage = errorMessage
            };
        }
    }
}