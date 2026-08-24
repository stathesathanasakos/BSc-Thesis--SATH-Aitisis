using SATH.Aitisis.Domain.Enums;

namespace SATH.Aitisis.App.Services.Notifications
{
    public sealed class CreateNotificationRequest
    {
        public string UserId { get; init; } = string.Empty;

        public int? ApplicationId { get; init; }

        public string Title { get; init; } = string.Empty;

        public string Message { get; init; } = string.Empty;

        public NotificationType Type { get; init; } = NotificationType.Info;
    }
}