namespace SATH.Aitisis.App.Services.Payments
{
    public interface IPaymentGateway
    {
        // adapter abstraction 
        Task<PaymentCheckoutResult> CreateCheckoutSessionAsync(PaymentCheckoutRequest request);

        Task<PaymentGatewaySessionStatus> GetSessionStatusAsync(string externalSessionId);
    }
}
