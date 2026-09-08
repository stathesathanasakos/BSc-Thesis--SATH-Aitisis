using SATH.Aitisis.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace SATH.Aitisis.Domain.Entities
{
    public class Application
    {
        public int Id { get; set; }

        public string CitizenId { get; set; } = string.Empty;

        public int ApplicationTypeId { get; set; }

        public string? ReviewedByUserId { get; set; }

        // Assigned only when the application is approved
        public string? ProtocolNumber { get; set; }

        // New applications always start in the draft state
        public ApplicationStatus Status { get; set; } = ApplicationStatus.Draft;
        //public ApplicationStatus Status { get; set; } = ApplicationStatus.Submitted;

        public string? Description { get; set; }

        public DateTime SubmissionDate { get; set; } = DateTime.UtcNow;

        public DateTime? DecisionDate { get; set; }

        public ApplicationType ApplicationType { get; set; } = null!;

        public ICollection<Document> Documents { get; set; } = new List<Document>();

        public ICollection<UserNotification> UserNotifications { get; set; } = new List<UserNotification>();

        public Payment? Payment { get; set; }

        public ICollection<ApplicationFieldValue> FieldValues { get; set; } = new List<ApplicationFieldValue>();
     }
}
