using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SATH.Aitisis.Domain.Enums;
using static System.Net.Mime.MediaTypeNames;

namespace SATH.Aitisis.Domain.Entities
{
    public class ApplicationType
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }

        public bool RequiresPayment { get; set; }

        public decimal? FeeAmount { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<Application> Applications { get; set; } = new List<Application>();

        public ICollection<ApplicationTypeField> Fields { get; set; } = new List<ApplicationTypeField>();

        public ICollection<DocumentRequirement> DocumentRequirements { get; set; } = new List<DocumentRequirement>();
    }
}
