using SATH.Aitisis.Domain.Enums;

namespace SATH.Aitisis.App.Services.ApplicationTypes
{
    public sealed class ApplicationTypeDetails
    {
        public int Id { get; init; }

        public string Name { get; init; } = string.Empty;

        public string? Description { get; init; }

        public bool RequiresPayment { get; init; }

        public decimal? FeeAmount { get; init; }

        public bool IsActive { get; init; }

        public IReadOnlyCollection<ApplicationTypeFieldItem> Fields { get; init; }
            = Array.Empty<ApplicationTypeFieldItem>();

        public IReadOnlyCollection<DocumentRequirementItem> DocumentRequirements { get; init; }
            = Array.Empty<DocumentRequirementItem>();
    }

    public sealed class ApplicationTypeFieldItem
    {
        public int Id { get; init; }

        public string Name { get; init; } = string.Empty;

        public string Label { get; init; } = string.Empty;

        public ApplicationFieldType FieldType { get; init; }

        public bool IsRequired { get; init; }

        public int DisplayOrder { get; init; }
    }

    public sealed class DocumentRequirementItem
    {
        public int Id { get; init; }

        public string Name { get; init; } = string.Empty;

        public string? Description { get; init; }

        public bool IsRequired { get; init; }

        public int DisplayOrder { get; init; }
    }
}
