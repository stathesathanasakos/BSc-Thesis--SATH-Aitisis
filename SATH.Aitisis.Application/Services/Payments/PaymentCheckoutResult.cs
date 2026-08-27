namespace SATH.Aitisis.App.Services.Payments
{
    public sealed class PaymentCheckoutResult
    {
        public bool Succeeded { get; init; }

        public string? ExternalSessionId { get; init; }

        public string? CheckoutUrl { get; init; }

        public string? ErrorMessage { get; init; }

        public static PaymentCheckoutResult Success(string externalSessionId, string checkoutUrl)
        {
            return new PaymentCheckoutResult
            {
                Succeeded = true,
                ExternalSessionId = externalSessionId,
                CheckoutUrl = checkoutUrl
            };
        }

        public static PaymentCheckoutResult Failure(string errorMessage)
        {
            return new PaymentCheckoutResult
            {
                Succeeded = false,
                ErrorMessage = errorMessage
            };
        }
    }
}
