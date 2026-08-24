using SATH.Aitisis.Domain.Entities;

namespace SATH.Aitisis.App.Services.Notifications
{
    public sealed class NotificationService : INotificationService
    {
        private readonly INotificationRepository _repository;

        public NotificationService(INotificationRepository repository)
        {
            _repository = repository;
        }

        public async Task CreateAsync(CreateNotificationRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.UserId))
            {
                return;
            }

            if (string.IsNullOrWhiteSpace(request.Title) || string.IsNullOrWhiteSpace(request.Message))
            {
                return;
            }

            var notification = new UserNotification
            {
                UserId = request.UserId,
                ApplicationId = request.ApplicationId,
                Title = request.Title.Trim(),
                Message = request.Message.Trim(),
                Type = request.Type,
                IsRead = false,
                CreatedAt = DateTime.UtcNow
            };

            await _repository.AddAsync(notification);
            await _repository.SaveChangesAsync();
        }

        public async Task<IReadOnlyCollection<NotificationListItem>> GetUserNotificationsAsync(string userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
            {
                return Array.Empty<NotificationListItem>();
            }

            var notifications = await _repository.GetByUserIdAsync(userId);

            return notifications
                .OrderByDescending(x => x.CreatedAt)
                .Select(x => new NotificationListItem
                {
                    Id = x.Id,
                    ApplicationId = x.ApplicationId,
                    Title = x.Title,
                    Message = x.Message,
                    Type = x.Type,
                    IsRead = x.IsRead,
                    CreatedAt = x.CreatedAt
                })
                .ToArray();
        }

        public async Task MarkAsReadAsync(int notificationId, string userId)
        {
            if (notificationId <= 0 || string.IsNullOrWhiteSpace(userId))
            {
                return;
            }

            var notification = await _repository.GetByIdAndUserIdForUpdateAsync(notificationId, userId);

            if (notification == null)
            {
                return;
            }

            if (notification.IsRead)
            {
                return;
            }

            notification.IsRead = true;

            await _repository.SaveChangesAsync();
        }
    }
}