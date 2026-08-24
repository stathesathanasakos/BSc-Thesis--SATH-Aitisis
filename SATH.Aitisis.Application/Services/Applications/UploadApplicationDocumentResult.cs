namespace SATH.Aitisis.App.Services.Applications
{
    public sealed class UploadApplicationDocumentResult
    {
        public bool Succeeded { get; init; }

        public IReadOnlyCollection<string> Errors { get; init; } = Array.Empty<string>();
    }
}