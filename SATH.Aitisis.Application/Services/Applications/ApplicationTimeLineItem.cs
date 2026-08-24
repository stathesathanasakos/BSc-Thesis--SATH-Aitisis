namespace SATH.Aitisis.App.Services.Applications
{
    public sealed class ApplicationTimelineItem
    {
        public string Title { get; init; } = string.Empty;

        public string Description { get; init; } = string.Empty;

        public DateTime OccurredAt { get; init; }

        public string CssClass { get; init; } = "secondary";
    }
}
