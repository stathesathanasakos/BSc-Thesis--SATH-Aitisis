using SATH.Aitisis.Domain.Entities;

namespace SATH.Aitisis.App.Services.Payments
{
    public interface IPaymentRepository
    {
        Task<Application?> GetApplicationForPaymentAsync(int applicationId, string citizenId);

        Task<Payment?> GetPaymentByApplicationIdAsync(int applicationId);

        Task<Payment?> GetPaymentByExternalSessionIdForUpdateAsync(string externalSessionId);

        Task AddPaymentAsync(Payment payment);

        Task SaveChangesAsync();
    }
}
