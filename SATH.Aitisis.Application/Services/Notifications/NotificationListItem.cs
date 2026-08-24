using SATH.Aitisis.Domain.Enums;

namespace SATH.Aitisis.App.Services.Notifications
{
    public sealed class NotificationListItem
    {
        public int Id { get; init; }

        public int? ApplicationId { get; init; }

        public string Title { get; init; } = string.Empty;

        public string Message { get; init; } = string.Empty;

        public NotificationType Type { get; init; }

        public bool IsRead { get; init; }

        public DateTime CreatedAt { get; init; }
    }
}