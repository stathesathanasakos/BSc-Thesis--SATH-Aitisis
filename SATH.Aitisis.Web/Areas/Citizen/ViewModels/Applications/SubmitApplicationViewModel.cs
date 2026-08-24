using System.ComponentModel.DataAnnotations;
using SATH.Aitisis.Domain.Enums;

namespace SATH.Aitisis.Web.Areas.Citizen.ViewModels.Applications
{
    public sealed class SubmitApplicationViewModel
    {
        public int ApplicationTypeId { get; set; }

        public string ApplicationTypeName { get; set; } = string.Empty;

        public string? ApplicationTypeDescription { get; set; }

        public bool RequiresPayment { get; set; }

        public decimal? FeeAmount { get; set; }

        [StringLength(1000)]
        public string? Description { get; set; }

        public List<ApplicationFieldInputViewModel> Fields { get; set; } = new();

        public List<ApplicationDocumentRequirementViewModel> DocumentRequirements{ get; set; } = new();
    }


    public sealed class ApplicationFieldInputViewModel
    {
        public int FieldId { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Label { get; set; } = string.Empty;

        public ApplicationFieldType FieldType { get; set; }

        public bool IsRequired { get; set; }

        public int DisplayOrder { get; set; }

        public string? Value { get; set; }
    }


    public sealed class ApplicationDocumentRequirementViewModel
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }

        public bool IsRequired { get; set; }

        public int DisplayOrder { get; set; }
    }
}