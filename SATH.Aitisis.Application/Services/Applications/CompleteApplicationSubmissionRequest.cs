namespace SATH.Aitisis.App.Services.Applications
{
    public sealed class CompleteApplicationSubmissionRequest
    {
        public int ApplicationId { get; init; }

        public string CitizenId { get; init; } = string.Empty;
    }
}