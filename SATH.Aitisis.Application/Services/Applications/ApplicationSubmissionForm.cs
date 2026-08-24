using SATH.Aitisis.Domain.Enums;

namespace SATH.Aitisis.App.Services.Applications
{
    public sealed class ApplicationSubmissionForm
    {
        public int ApplicationTypeId { get; init; }

        public string ApplicationTypeName { get; init; } = string.Empty;

        public string? Description { get; init; }

        public bool RequiresPayment { get; init; }

        public decimal? FeeAmount { get; init; }

        public IReadOnlyCollection<ApplicationFormFieldItem> Fields { get; init; }
            = Array.Empty<ApplicationFormFieldItem>();

        public IReadOnlyCollection<ApplicationDocumentRequirementItem> DocumentRequirements { get; init; }
            = Array.Empty<ApplicationDocumentRequirementItem>();
    }

    public sealed class ApplicationFormFieldItem
    {
        public int Id { get; init; }

        public string Name { get; init; } = string.Empty;

        public string Label { get; init; } = string.Empty;

        public ApplicationFieldType FieldType { get; init; }

        public bool IsRequired { get; init; }

        public int DisplayOrder { get; init; }
    }

    public sealed class ApplicationDocumentRequirementItem
    {
        public int Id { get; init; }

        public string Name { get; init; } = string.Empty;

        public string? Description { get; init; }

        public bool IsRequired { get; init; }

        public int DisplayOrder { get; init; }
    }
}