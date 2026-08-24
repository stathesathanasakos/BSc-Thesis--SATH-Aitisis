using SATH.Aitisis.Domain.Enums;

namespace SATH.Aitisis.App.Services.Applications
{
    public sealed class ApplicationTimelineDetails
    {
        public int ApplicationId { get; init; }

        public string ApplicationTypeName { get; init; } = string.Empty;

        public ApplicationStatus Status { get; init; }

        public string? ProtocolNumber { get; init; }

        public IReadOnlyCollection<ApplicationTimelineItem> Items { get; init; } = Array.Empty<ApplicationTimelineItem>();
    }
}