namespace SATH.Aitisis.App.Services.Applications
{
    public sealed class PendingApplicationItem
    {
        public int Id { get; init; }

        public string ApplicationTypeName { get; init; }
            = string.Empty;

        public DateTime SubmissionDate { get; init; }
    }
}