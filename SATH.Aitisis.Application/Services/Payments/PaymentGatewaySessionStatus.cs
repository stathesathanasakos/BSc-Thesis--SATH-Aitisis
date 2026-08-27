namespace SATH.Aitisis.App.Services.Payments
{
    public sealed class PaymentGatewaySessionStatus
    {
        public bool Exists { get; init; }

        public bool IsPaid { get; init; }

        public string? ExternalSessionId { get; init; }

        public string? ExternalPaymentId { get; init; }

        public string? RawStatus { get; init; }

        public static PaymentGatewaySessionStatus NotFound()
        {
            return new PaymentGatewaySessionStatus
            {
                Exists = false,
                IsPaid = false
            };
        }

        public static PaymentGatewaySessionStatus Paid(string externalSessionId, string? externalPaymentId, string? rawStatus)
        {
            return new PaymentGatewaySessionStatus
            {
                Exists = true,
                IsPaid = true,
                ExternalSessionId = externalSessionId,
                ExternalPaymentId = externalPaymentId,
                RawStatus = rawStatus
            };
        }

        public static PaymentGatewaySessionStatus Unpaid(string externalSessionId, string? rawStatus)
        {
            return new PaymentGatewaySessionStatus
            {
                Exists = true,
                IsPaid = false,
                ExternalSessionId = externalSessionId,
                RawStatus = rawStatus
            };
        }
    }
}
