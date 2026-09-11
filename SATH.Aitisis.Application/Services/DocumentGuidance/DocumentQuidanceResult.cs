namespace SATH.Aitisis.App.Services.DocumentGuidance
{
    public sealed class DocumentGuidanceResult
    {
        public bool Succeeded { get; init; }

        public string Summary { get; init; } = string.Empty;

        public IReadOnlyCollection<DocumentGuidanceItem> Items { get; init; } = Array.Empty<DocumentGuidanceItem>();

        public string GeneralAdvice { get; init; } = string.Empty;

        public string Disclaimer { get; init; } = string.Empty;

        public string? ErrorMessage { get; init; }

        public static DocumentGuidanceResult Success(string summary, IReadOnlyCollection<DocumentGuidanceItem> items, string generalAdvice, string disclaimer)
        {
            return new DocumentGuidanceResult
            {
                Succeeded = true,
                Summary = summary,
                Items = items,
                GeneralAdvice = generalAdvice,
                Disclaimer = disclaimer
            };
        }

        public static DocumentGuidanceResult Failure(string errorMessage)
        {
            return new DocumentGuidanceResult
            {
                Succeeded = false,
                ErrorMessage = errorMessage
            };
        }
    }


    public sealed class DocumentGuidanceItem
    {
        public string DocumentName { get; init; } = string.Empty;

        public IReadOnlyCollection<string> Steps { get; init; }= Array.Empty<string>();
    }
}