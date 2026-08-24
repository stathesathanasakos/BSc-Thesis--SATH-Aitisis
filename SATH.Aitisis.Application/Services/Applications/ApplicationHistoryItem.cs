using SATH.Aitisis.Domain.Enums;

namespace SATH.Aitisis.App.Services.Applications
{
    public sealed class ApplicationHistoryItem
    {
        public int Id { get; init; }

        public string ApplicationTypeName { get; init; } = string.Empty;

        public ApplicationStatus Status { get; init; }

        public DateTime SubmissionDate { get; init; }

        public string? ProtocolNumber { get; init; }

        public DateTime? DecisionDate { get; init; }
    }
}