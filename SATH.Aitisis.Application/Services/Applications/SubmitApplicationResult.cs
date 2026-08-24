namespace SATH.Aitisis.App.Services.Applications
{
    public sealed class SubmitApplicationResult
    {
        public bool Succeeded { get; init; }

        public int? ApplicationId { get; init; }

        public IReadOnlyCollection<string> Errors { get; init; } = Array.Empty<string>();
    }
}
