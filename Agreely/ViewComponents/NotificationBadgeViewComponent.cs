using Agreely.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Agreely.ViewComponents
{
    public class NotificationBadgeViewComponent : ViewComponent
    {
        private readonly INotificationService _notificationService;

        public NotificationBadgeViewComponent(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        public IViewComponentResult Invoke()
        {
            var userIdValue = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userIdValue == null || !int.TryParse(userIdValue, out int userId))
                return View(0);

            var count = _notificationService.GetUnreadCount(userId);
            return View(count);
        }
    }
}