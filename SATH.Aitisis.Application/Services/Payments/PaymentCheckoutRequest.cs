namespace SATH.Aitisis.App.Services.Payments
{
    public sealed class PaymentCheckoutRequest
    {
        public int ApplicationId { get; init; }

        public int PaymentId { get; init; }

        public string CitizenId { get; init; } = string.Empty;

        public decimal Amount { get; init; }

        public string Currency { get; init; } = "eur";

        public string Description { get; init; } = string.Empty;

        public string SuccessUrl { get; init; } = string.Empty;

        public string CancelUrl { get; init; } = string.Empty;
    }
}