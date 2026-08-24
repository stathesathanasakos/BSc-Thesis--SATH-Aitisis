using SATH.Aitisis.Domain.Enums;

namespace SATH.Aitisis.App.Services.ApplicationTypes
{
    public sealed class AddApplicationTypeFieldRequest
    {
        public int ApplicationTypeId { get; init; }

        public string Name { get; init; } = string.Empty;

        public string Label { get; init; } = string.Empty;

        public ApplicationFieldType FieldType { get; init; }

        public bool IsRequired { get; init; }
    }
}