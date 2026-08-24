namespace SATH.Aitisis.App.Services.Applications
{
    public sealed class ApplicationDocumentsInfo
    {
        public int ApplicationId { get; init; }

        public int ApplicationTypeId { get; init; }

        public string ApplicationTypeName { get; init; } = string.Empty;

        public bool RequiresPayment { get; init; }

        public IReadOnlyCollection<ApplicationDocumentStatusItem>Documents{ get; init; } = Array.Empty<ApplicationDocumentStatusItem>();

        public bool RequiredDocumentsCompleted => Documents
                .Where(x => x.IsRequired)
                .All(x => x.IsUploaded);
    }

    public sealed class ApplicationDocumentStatusItem
    {
        public int DocumentRequirementId { get; init; }

        public string Name { get; init; } = string.Empty;

        public string? Description { get; init; }

        public bool IsRequired { get; init; }

        public int DisplayOrder { get; init; }

        public bool IsUploaded { get; init; }
    }
}