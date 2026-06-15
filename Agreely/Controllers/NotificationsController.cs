using Agreely.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Agreely.Controllers
{
    public class NotificationsController : BaseController
    {
        private readonly INotificationService _notificationService;

        public NotificationsController(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var userId = GetSessionUserId();
            var notifications = _notificationService.GetNotificationsForUser(userId);
            return View(notifications);
        }

        [HttpPost]
        public IActionResult MarkAsRead(int notificationId)
        {
            _notificationService.MarkAsRead(notificationId);
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Open(int notificationId)
        {
            var userId = GetSessionUserId();
            var n = _notificationService.GetByIdForUser(notificationId, userId);

            if (n == null)
                return NotFound();

            if (!n.IsRead)
                _notificationService.MarkAsRead(notificationId);

            var url = Url.Action("Details", "Group", new { groupId = n.GroupId })
              + $"#commitment-{n.CommitmentId}";
            return Redirect(url);
        }
    }
}