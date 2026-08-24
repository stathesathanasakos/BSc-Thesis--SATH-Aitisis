using Microsoft.EntityFrameworkCore;
using SATH.Aitisis.App.Services.Notifications;
using SATH.Aitisis.Domain.Entities;
using SATH.Aitisis.Infrastructure.Persistence;

namespace SATH.Aitisis.Infrastructure.Repositories.Notifications
{
    public sealed class NotificationRepository : INotificationRepository
    {
        private readonly SathAitisisDbContext _dbContext;

        public NotificationRepository(SathAitisisDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task AddAsync(UserNotification notification)
        {
            await _dbContext.UserNotifications.AddAsync(notification);
        }

        public async Task<IReadOnlyCollection<UserNotification>> GetByUserIdAsync(string userId)
        {
            return await _dbContext.UserNotifications
                .AsNoTracking()
                .Where(x => x.UserId == userId)
                .OrderByDescending(x => x.CreatedAt)
                .ToArrayAsync();
        }

        public async Task<UserNotification?> GetByIdAndUserIdForUpdateAsync(int notificationId, string userId)
        {
            return await _dbContext.UserNotifications.FirstOrDefaultAsync(x => x.Id == notificationId && x.UserId == userId);
        }

        public async Task SaveChangesAsync()
        {
            await _dbContext.SaveChangesAsync();
        }
    }
}