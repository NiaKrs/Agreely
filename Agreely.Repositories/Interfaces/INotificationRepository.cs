
using Agreely.Domain;
using Agreely.Domain.Enums;

namespace Agreely.Repositories.Interfaces
{
    public interface INotificationRepository
    {
        int CreateNotification(Notification notification);
        List<Notification> GetNotificationsByUserId(int userId);
        void MarkAsRead(int notificationId);
        int GetUnreadCount(int userId);
        bool NotificationExists(int userId, int commitmentId, HealthStatusValue healthStatus);
        Notification? GetNotificationByIdForUser(int notificationId, int userId);
    }
}
