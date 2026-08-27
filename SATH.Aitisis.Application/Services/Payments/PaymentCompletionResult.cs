namespace SATH.Aitisis.App.Services.Payments
{
    public sealed class PaymentCompletionResult
    {
        public bool Succeeded { get; init; }

        public int? ApplicationId { get; init; }

        public string? ErrorMessage { get; init; }

        public static PaymentCompletionResult Success(int applicationId)
        {
            return new PaymentCompletionResult
            {
                Succeeded = true,
                ApplicationId = applicationId
            };
        }

        public static PaymentCompletionResult Failure(string errorMessage)
        {
            return new PaymentCompletionResult
            {
                Succeeded = false,
                ErrorMessage = errorMessage
            };
        }
    }
}