using Agreely.Domain.Enums;
using Agreely.Services.DTO.Responses;

namespace Agreely.Services.Interfaces
{
    public interface INotificationService
    {
        void CreateNotificationsForCommitment(
            int commitmentId, int groupId, HealthStatusValue healthStatus, string commitmentTitle);
        List<NotificationResponse> GetNotificationsForUser(int userId);
        int GetUnreadCount(int userId);
        void MarkAsRead(int notificationId);
        NotificationResponse? GetByIdForUser(int notificationId, int userId);
    }
}
