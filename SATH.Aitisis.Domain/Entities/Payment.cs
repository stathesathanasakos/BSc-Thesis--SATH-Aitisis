using SATH.Aitisis.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SATH.Aitisis.Domain.Entities
{
    public class Payment
    {
        public int Id { get; set; }

        public int ApplicationId { get; set; }

        public decimal Amount { get; set; }

        public PaymentStatus Status { get; set; }
            = PaymentStatus.Pending;

        public string Provider { get; set; } = string.Empty;

        public string? ExternalPaymentId { get; set; }

        public DateTime? PaymentDate { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public Application Application { get; set; } = null!;
    }
}
