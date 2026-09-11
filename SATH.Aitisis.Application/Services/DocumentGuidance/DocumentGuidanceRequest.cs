namespace SATH.Aitisis.App.Services.DocumentGuidance
{
    public sealed class DocumentGuidanceRequest
    {
        public int ApplicationId { get; init; }

        public string ApplicationTypeName { get; init; } = string.Empty;

        public IReadOnlyCollection<DocumentGuidanceDocumentItem> Documents { get; init; } = Array.Empty<DocumentGuidanceDocumentItem>();
    }
}