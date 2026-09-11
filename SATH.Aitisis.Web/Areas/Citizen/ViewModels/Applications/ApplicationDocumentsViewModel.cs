namespace SATH.Aitisis.Web.Areas.Citizen.ViewModels.Applications
{
    public sealed class ApplicationDocumentsViewModel
    {
        public int ApplicationId { get; set; }

        public int ApplicationTypeId { get; set; }

        public string ApplicationTypeName { get; set; } = string.Empty;

        public bool RequiresPayment { get; set; }

        public bool RequiredDocumentsCompleted { get; set; }

        public List<DocumentRequirementUploadItemViewModel> DocumentRequirements { get; set; } = new();

        public AiDocumentGuidanceViewModel? AiDocumentGuidance { get; set; }
    }

    public sealed class DocumentRequirementUploadItemViewModel
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }

        public bool IsRequired { get; set; }

        public int DisplayOrder { get; set; }

        public bool IsUploaded { get; set; }
    }

    public sealed class AiDocumentGuidanceViewModel
    {
        public string Summary { get; set; } = string.Empty;

        public List<AiDocumentGuidanceItemViewModel> Items { get; set; }
            = new();

        public string GeneralAdvice { get; set; } = string.Empty;

        public string Disclaimer { get; set; } = string.Empty;
    }

    public sealed class AiDocumentGuidanceItemViewModel
    {
        public string DocumentName { get; set; } = string.Empty;

        public List<string> Steps { get; set; } = new();
    }
}