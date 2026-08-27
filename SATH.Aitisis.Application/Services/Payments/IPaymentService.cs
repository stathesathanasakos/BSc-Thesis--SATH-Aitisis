namespace SATH.Aitisis.App.Services.Payments
{
    public interface IPaymentService
    {
        // web controller uses this interface
        Task<PaymentStartResult> StartPaymentAsync(StartPaymentRequest request);

        Task<PaymentCompletionResult> CompletePaymentAsync(CompletePaymentRequest request);

        Task<PaymentDetails?> GetPaymentDetailsAsync(int applicationId, string citizenId);
    }
}
