namespace SATH.Aitisis.Domain.Entities
{
    // Defines a supporting PDF document required by an application type
    public class DocumentRequirement
    {
        public int Id { get; set; }

        public int ApplicationTypeId { get; set; }

        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }

        public bool IsRequired { get; set; }

        public int DisplayOrder { get; set; }

        public ApplicationType ApplicationType { get; set; } = null!;

        public ICollection<Document> Documents { get; set; }
            = new List<Document>();
    }
}