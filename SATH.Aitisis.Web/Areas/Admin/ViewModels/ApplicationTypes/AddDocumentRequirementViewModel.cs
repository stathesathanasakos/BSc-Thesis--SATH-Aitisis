using System.ComponentModel.DataAnnotations;

namespace SATH.Aitisis.Web.Areas.Admin.ViewModels.ApplicationTypes
{
    public sealed class AddDocumentRequirementViewModel
    {
        public int ApplicationTypeId { get; set; }

        [Required]
        [StringLength(150)]
        public string Name { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Description { get; set; }

        [Display(Name = "Required")]
        public bool IsRequired { get; set; }
    }
}