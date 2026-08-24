namespace SATH.Aitisis.App.Services.Applications
{
    public sealed class ApplicationDocumentFile
    {
        public Stream Content { get; init; } = Stream.Null;

        public string ContentType { get; init; } = "application/pdf";

        public string FileName { get; init; } = string.Empty;
    }
}