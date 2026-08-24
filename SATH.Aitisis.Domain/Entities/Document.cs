using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SATH.Aitisis.Domain.Entities
{
    //This entity stores metadata for a PDF document attached to an application
    //The actual file is stored outside the database in filesystem
    public class Document
    {
        public int Id { get; set; }

        public int ApplicationId { get; set; }

        public string UploadedByUserId { get; set; } = string.Empty;

        public string OriginalFileName { get; set; } = string.Empty;

        // Server-generated filename used for physical storage
        public string StoredFileName { get; set; } = string.Empty;

        public string ContentType { get; set; } = string.Empty;

        public long FileSize { get; set; }

        public DateTime UploadDate { get; set; } = DateTime.UtcNow;

        public int? DocumentRequirementId { get; set; }

        public DocumentRequirement? DocumentRequirement { get; set; }

        public Application Application { get; set; } = null!;
    }
}
