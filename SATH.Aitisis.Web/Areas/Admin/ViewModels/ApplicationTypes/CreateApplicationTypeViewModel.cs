using System.ComponentModel.DataAnnotations;

namespace SATH.Aitisis.Web.Areas.Admin.ViewModels.ApplicationTypes
{
    public sealed class CreateApplicationTypeViewModel
    {
        [Required]
        [StringLength(150)]
        [Display(Name = "Application Type Name")]
        public string Name { get; set; } = string.Empty;

        [StringLength(1000)]
        [Display(Name = "Description")]
        public string? Description { get; set; }

        [Display(Name = "Requires Payment")]
        public bool RequiresPayment { get; set; }

        [Range(0.01, 100000)]
        [Display(Name = "Fee Amount")]
        public decimal? FeeAmount { get; set; }
    }
}