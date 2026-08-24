namespace SATH.Aitisis.Domain.Entities
{
    // Stores the value submitted by a citizen for a dynamic application field

    public class ApplicationFieldValue
    {
        public int Id { get; set; }

        public int ApplicationId { get; set; }

        public int ApplicationTypeFieldId { get; set; }

        public string Value { get; set; } = string.Empty;

        public Application Application { get; set; } = null!;

        public ApplicationTypeField ApplicationTypeField { get; set; } = null!;
    }
}