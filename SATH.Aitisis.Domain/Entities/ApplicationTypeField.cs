using SATH.Aitisis.Domain.Enums;

namespace SATH.Aitisis.Domain.Entities
{
    // Defines a dynamic input field required by a specific application type
    public class ApplicationTypeField
    {
        public int Id { get; set; }

        public int ApplicationTypeId { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Label { get; set; } = string.Empty;

        public ApplicationFieldType FieldType { get; set; }

        public bool IsRequired { get; set; }

        public int DisplayOrder { get; set; }

        public ApplicationType ApplicationType { get; set; } = null!;

        public ICollection<ApplicationFieldValue> Values { get; set; }
            = new List<ApplicationFieldValue>();
    }
}