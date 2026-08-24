namespace SATH.Aitisis.App.Services.Applications
{
    public sealed class ApplicationReviewDetails
    {
        public int Id { get; init; }

        public string ApplicationTypeName { get; init; } = string.Empty;

        public string CitizenId { get; init; } = string.Empty;

        public string CitizenFirstName { get; init; } = string.Empty;

        public string CitizenLastName { get; init; } = string.Empty;

        public string CitizenEmail { get; init; } = string.Empty;

        public DateTime SubmissionDate { get; init; }

        public string Status { get; init; } = string.Empty;

        public string? Description { get; init; }

        public IReadOnlyCollection<ApplicationReviewFieldItem> Fields { get; init; } = Array.Empty<ApplicationReviewFieldItem>();

        public IReadOnlyCollection<ApplicationReviewDocumentItem> Documents { get; init; } = Array.Empty<ApplicationReviewDocumentItem>();
    }


    public sealed class ApplicationReviewFieldItem
    {
        public int FieldId { get; init; }

        public string Label { get; init; } = string.Empty;

        public string Value { get; init; }  = string.Empty;

        public int DisplayOrder { get; init; }
    }


    public sealed class ApplicationReviewDocumentItem
    {
        public int DocumentId { get; init; }

        public string RequirementName { get; init; } = string.Empty;

        public string OriginalFileName { get; init; } = string.Empty;

        public DateTime UploadDate { get; init; }

        public bool IsRequired { get; init; }
    }
}