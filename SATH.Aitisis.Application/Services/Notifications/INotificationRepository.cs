using SATH.Aitisis.Domain.Entities;

namespace SATH.Aitisis.App.Services.Notifications
{
    public interface INotificationRepository
    {
        Task AddAsync(UserNotification notification);

        Task<IReadOnlyCollection<UserNotification>> GetByUserIdAsync(string userId);

        Task<UserNotification?> GetByIdAndUserIdForUpdateAsync(int notificationId,string userId);

        Task SaveChangesAsync();
    }
}