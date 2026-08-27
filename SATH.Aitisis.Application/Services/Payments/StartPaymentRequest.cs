namespace SATH.Aitisis.App.Services.Payments
{
    public sealed class StartPaymentRequest
    {
        public int ApplicationId { get; init; }

        public string CitizenId { get; init; } = string.Empty;

        public string SuccessUrl { get; init; } = string.Empty;

        public string CancelUrl { get; init; } = string.Empty;
    }
}