using SATH.Aitisis.Domain.Enums;

namespace SATH.Aitisis.App.Services.Payments
{
    public sealed class PaymentDetails
    {
        public int ApplicationId { get; init; }

        public string ApplicationTypeName { get; init; } = string.Empty;

        public decimal Amount { get; init; }

        public string Currency { get; init; } = "eur";

        public PaymentStatus? PaymentStatus { get; init; }

        public bool RequiresPayment { get; init; }

        public bool CanStartPayment { get; init; }
    }
}
