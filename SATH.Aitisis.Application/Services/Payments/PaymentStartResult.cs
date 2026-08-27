namespace SATH.Aitisis.App.Services.Payments
{
    public sealed class PaymentStartResult
    {
        public bool Succeeded { get; init; }

        public string? CheckoutUrl { get; init; }

        public string? ErrorMessage { get; init; }

        public static PaymentStartResult Success( string checkoutUrl)
        {
            return new PaymentStartResult
            {
                Succeeded = true,
                CheckoutUrl = checkoutUrl
            };
        }

        public static PaymentStartResult Failure(string errorMessage)
        {
            return new PaymentStartResult
            {
                Succeeded = false,
                ErrorMessage = errorMessage
            };
        }
    }
}