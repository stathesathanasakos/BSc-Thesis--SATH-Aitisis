namespace SATH.Aitisis.App.Services.Payments
{
    public sealed class CompletePaymentRequest
    {
        public string ExternalSessionId { get; init; } = string.Empty;

        public string CitizenId { get; init; } = string.Empty;
    }
}