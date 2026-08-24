using System.ComponentModel.DataAnnotations;

namespace SATH.Aitisis.Web.Areas.Citizen.ViewModels.Applications
{
    public sealed class UploadDocumentViewModel
    {
        public int ApplicationId { get; set; }

        public int ApplicationTypeId { get; set; }

        public int? DocumentRequirementId { get; set; }

        [Required]
        public IFormFile? File { get; set; }
    }
}