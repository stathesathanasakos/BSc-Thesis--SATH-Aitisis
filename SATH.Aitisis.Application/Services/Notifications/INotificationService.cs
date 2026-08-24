namespace SATH.Aitisis.App.Services.Notifications
{
    public interface INotificationService
    {
        Task CreateAsync(CreateNotificationRequest request);

        Task<IReadOnlyCollection<NotificationListItem>> GetUserNotificationsAsync(string userId);

        Task MarkAsReadAsync(int notificationId, string userId);
    }
}