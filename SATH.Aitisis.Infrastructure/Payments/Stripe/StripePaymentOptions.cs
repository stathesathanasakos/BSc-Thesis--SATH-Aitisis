namespace SATH.Aitisis.Infrastructure.Payments.Stripe
{
    public sealed class StripePaymentOptions
    {
        public string SecretKey { get; init; } = string.Empty;

        public string Currency { get; init; } = "eur";
    }
}