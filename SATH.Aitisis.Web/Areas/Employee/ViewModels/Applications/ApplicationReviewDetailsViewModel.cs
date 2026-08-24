namespace SATH.Aitisis.Web.Areas.Employee.ViewModels.Applications
{
    public sealed class ApplicationReviewDetailsViewModel
    {
        public int Id { get; set; }

        public string ApplicationTypeName { get; set; } = string.Empty;

        public string CitizenFirstName { get; set; } = string.Empty;

        public string CitizenLastName { get; set; } = string.Empty;

        public string CitizenEmail { get; set; } = string.Empty;

        public DateTime SubmissionDate { get; set; }

        public string Status { get; set; } = string.Empty;

        public string? Description { get; set; }

        public List<ApplicationReviewFieldViewModel> Fields { get; set; } = new();

        public List<ApplicationReviewDocumentViewModel> Documents { get; set; } = new();
    }


    public sealed class ApplicationReviewFieldViewModel
    {
        public string Label { get; set; } = string.Empty;

        public string Value { get; set; } = string.Empty;

        public int DisplayOrder { get; set; }
    }


    public sealed class ApplicationReviewDocumentViewModel
    {
        public int DocumentId { get; set; }

        public string RequirementName { get; set; } = string.Empty;

        public string OriginalFileName { get; set; } = string.Empty;

        public DateTime UploadDate { get; set; }

        public bool IsRequired { get; set; }
    }
}