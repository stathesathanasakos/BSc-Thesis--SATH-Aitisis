using SATH.Aitisis.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SATH.Aitisis.Domain.Entities
{
    public class UserNotification
    {
        public int Id { get; set; }

        public string UserId { get; set; } = string.Empty;

        public int? ApplicationId { get; set; }

        public string Title { get; set; } = string.Empty;

        public string Message { get; set; } = string.Empty;

        public NotificationType Type { get; set; }
            = NotificationType.Info;

        public bool IsRead { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public Application? Application { get; set; }
    }
}
