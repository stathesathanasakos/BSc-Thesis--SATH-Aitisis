using SATH.Aitisis.Domain.Enums;

namespace SATH.Aitisis.Web.Areas.Citizen.ViewModels.Notifications
{
    public sealed class NotificationListItemViewModel
    {
        public int Id { get; set; }

        public int? ApplicationId { get; set; }

        public string Title { get; set; } = string.Empty;

        public string Message { get; set; } = string.Empty;

        public NotificationType Type { get; set; }

        public bool IsRead { get; set; }

        public DateTime CreatedAt { get; set; }

        public string BadgeText => IsRead ? "Read" : "Unread";

        public string BadgeCssClass => IsRead ? "bg-secondary" : "bg-primary";
    }
}