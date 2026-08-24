using SATH.Aitisis.Domain.Enums;

namespace SATH.Aitisis.Web.Areas.Admin.ViewModels.ApplicationTypes
{
    public sealed class ApplicationTypeDetailsViewModel
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }

        public bool RequiresPayment { get; set; }

        public decimal? FeeAmount { get; set; }

        public bool IsActive { get; set; }

        public List<ApplicationTypeFieldViewModel> Fields { get; set; } = new();

        public List<DocumentRequirementViewModel> DocumentRequirements { get; set; } = new();
    }

    public sealed class ApplicationTypeFieldViewModel
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Label { get; set; } = string.Empty;

        public ApplicationFieldType FieldType { get; set; }

        public bool IsRequired { get; set; }

        public int DisplayOrder { get; set; }
    }

    public sealed class DocumentRequirementViewModel
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }

        public bool IsRequired { get; set; }

        public int DisplayOrder { get; set; }
    }
}