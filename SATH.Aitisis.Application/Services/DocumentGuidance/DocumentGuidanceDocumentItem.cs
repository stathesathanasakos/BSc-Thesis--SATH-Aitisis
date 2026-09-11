namespace SATH.Aitisis.App.Services.DocumentGuidance
{
    public sealed class DocumentGuidanceDocumentItem
    {
        public string Name { get; init; } = string.Empty;

        public string? Description { get; init; }

        public bool IsRequired { get; init; }

        public int DisplayOrder { get; init; }
    }
}