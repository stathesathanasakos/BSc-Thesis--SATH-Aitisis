namespace SATH.Aitisis.App.Services.ApplicationTypes
{
    public sealed class AddDocumentRequirementRequest
    {
        public int ApplicationTypeId { get; init; }

        public string Name { get; init; } = string.Empty;

        public string? Description { get; init; }

        public bool IsRequired { get; init; }
    }
}