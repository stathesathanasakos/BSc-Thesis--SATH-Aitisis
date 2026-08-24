using SATH.Aitisis.Domain.Enums;

namespace SATH.Aitisis.Web.ViewModels.Applications
{
    public sealed class ApplicationTimelineViewModel
    {
        public int ApplicationId { get; set; }

        public string ApplicationTypeName { get; set; } = string.Empty;

        public ApplicationStatus Status { get; set; }

        public string? ProtocolNumber { get; set; }

        public List<ApplicationTimelineItemViewModel> Items { get; set; } = new();
    }

    public sealed class ApplicationTimelineItemViewModel
    {
        public string Title { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public DateTime OccurredAt { get; set; }

        public string CssClass { get; set; } = "secondary";
    }
}