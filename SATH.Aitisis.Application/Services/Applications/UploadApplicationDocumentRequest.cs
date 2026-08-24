namespace SATH.Aitisis.App.Services.Applications
{
    public sealed class UploadApplicationDocumentRequest
    {
        public int ApplicationId { get; init; }

        public string CitizenId { get; init; } = string.Empty;

        public int? DocumentRequirementId { get; init; }

        public string OriginalFileName { get; init; } = string.Empty;

        public string ContentType { get; init; } = string.Empty;

        public long FileSize { get; init; }

        public Stream Content { get; init; } = Stream.Null;
    }
}