namespace SATH.Aitisis.App.Services.Applications
{
    public sealed class SubmitApplicationRequest
    {
        public string CitizenId { get; init; } = string.Empty;

        public int ApplicationTypeId { get; init; }

        public string? Description { get; init; }

        public IReadOnlyCollection<SubmittedFieldValue> FieldValues { get; init; } = Array.Empty<SubmittedFieldValue>();
    }


    public sealed class SubmittedFieldValue
    {
        public int ApplicationTypeFieldId { get; init; }

        public string Value { get; init; } = string.Empty;
    }
}