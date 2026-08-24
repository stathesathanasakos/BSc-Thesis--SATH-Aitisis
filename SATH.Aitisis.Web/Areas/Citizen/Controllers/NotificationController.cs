using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SATH.Aitisis.App.Common.Constants;
using SATH.Aitisis.App.Services.Notifications;
using SATH.Aitisis.Web.Areas.Citizen.ViewModels.Notifications;
using System.Security.Claims;

namespace SATH.Aitisis.Web.Areas.Citizen.Controllers
{
    [Area("Citizen")]
    [Authorize(Roles = AppRoles.Citizen)]
    public sealed class NotificationsController : Controller
    {
        private readonly INotificationService _notificationService;

        public NotificationsController(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }


        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrWhiteSpace(userId))
            {
                return Unauthorized();
            }

            var notifications = await _notificationService.GetUserNotificationsAsync(userId);

            var model = notifications
                .Select(x => new NotificationListItemViewModel
                {
                    Id = x.Id,
                    ApplicationId = x.ApplicationId,
                    Title = x.Title,
                    Message = x.Message,
                    Type = x.Type,
                    IsRead = x.IsRead,
                    CreatedAt = x.CreatedAt
                })
                .ToList();

            return View(model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MarkAsRead(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrWhiteSpace(userId))
            {
                return Unauthorized();
            }

            await _notificationService.MarkAsReadAsync(id, userId);

            return RedirectToAction(nameof(Index));
        }
    }
}