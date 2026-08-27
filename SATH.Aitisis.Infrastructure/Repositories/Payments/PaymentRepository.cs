using Microsoft.EntityFrameworkCore;
using SATH.Aitisis.App.Services.Payments;
using SATH.Aitisis.Domain.Entities;
//using SATH.Aitisis.Infrastructure.Data;
using SATH.Aitisis.Infrastructure.Persistence;

namespace SATH.Aitisis.Infrastructure.Repositories.Payments
{
    public sealed class PaymentRepository : IPaymentRepository
    {
        private readonly SathAitisisDbContext _dbContext;

        public PaymentRepository(SathAitisisDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Application?> GetApplicationForPaymentAsync(int applicationId, string citizenId)
        {
            return await _dbContext.Applications
                .AsNoTracking()
                .Include(x => x.ApplicationType)
                .Include(x => x.Payment)
                .FirstOrDefaultAsync(x =>
                    x.Id == applicationId &&
                    x.CitizenId == citizenId);
        }

        public async Task<Payment?> GetPaymentByApplicationIdAsync(
            int applicationId)
        {
            return await _dbContext.Payments
                .FirstOrDefaultAsync(x =>
                    x.ApplicationId == applicationId);
        }

        public async Task<Payment?> GetPaymentByExternalSessionIdForUpdateAsync(string externalSessionId)
        {
            return await _dbContext.Payments
                .Include(x => x.Application)
                .FirstOrDefaultAsync(x => x.ExternalPaymentId == externalSessionId);
        }

        public async Task AddPaymentAsync(Payment payment)
        {
            await _dbContext.Payments.AddAsync(payment);
        }

        public async Task SaveChangesAsync()
        {
            await _dbContext.SaveChangesAsync();
        }
    }
}