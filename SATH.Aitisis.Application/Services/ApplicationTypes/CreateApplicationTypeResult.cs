namespace SATH.Aitisis.App.Services.ApplicationTypes
{
    public sealed class CreateApplicationTypeResult
    {
        public bool Succeeded { get; init; }

        public int? ApplicationTypeId { get; init; }

        public IReadOnlyCollection<string> Errors { get; init; }
            = Array.Empty<string>();
    }
}