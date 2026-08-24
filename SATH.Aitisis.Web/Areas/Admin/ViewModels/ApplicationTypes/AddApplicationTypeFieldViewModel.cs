using System.ComponentModel.DataAnnotations;
using SATH.Aitisis.Domain.Enums;

namespace SATH.Aitisis.Web.Areas.Admin.ViewModels.ApplicationTypes
{
    public sealed class AddApplicationTypeFieldViewModel
    {
        public int ApplicationTypeId { get; set; }

        [Required]
        [StringLength(100)]
        [Display(Name = "Technical Name")]
        public string Name { get; set; } = string.Empty;

        [Required]
        [StringLength(150)]
        [Display(Name = "Label")]
        public string Label { get; set; } = string.Empty;

        [Display(Name = "Field Type")]
        public ApplicationFieldType FieldType { get; set; }

        [Display(Name = "Required")]
        public bool IsRequired { get; set; }
    }
}