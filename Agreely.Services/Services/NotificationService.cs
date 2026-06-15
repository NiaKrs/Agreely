using Agreely.Domain;
using Agreely.Domain.Enums;
using Agreely.Repositories.Interfaces;
using Agreely.Services.DTO.Responses;
using Agreely.Services.Interfaces;

namespace Agreely.Services.Services
{
    public class NotificationService : INotificationService
    {
        private readonly INotificationRepository _notificationRepo;
        private readonly IGroupMembershipRepository _membershipRepo;
        private readonly IGroupRepository _groupRepo;

        public NotificationService(
            INotificationRepository notificationRepo,
            IGroupMembershipRepository membershipRepo,
            IGroupRepository groupRepo)
        {
            _notificationRepo = notificationRepo;
            _membershipRepo = membershipRepo;
            _groupRepo = groupRepo;
        }
        public void CreateNotificationsForCommitment(int commitmentId, int groupId, HealthStatusValue healthStatus, string commitmentTitle)
        {
            if (healthStatus == HealthStatusValue.Healthy)
                return;

            var groupName = _groupRepo.GetGroupNameById(groupId) ?? $"Group {groupId}";
            string prefix = $"[Group: {groupName}]";

            string message = healthStatus switch
            {
                HealthStatusValue.NeedsAttention =>
                    $"{prefix} . Commitment \"{commitmentTitle}\" has been pending for too long and needs attention.",

                HealthStatusValue.DueForReview =>
                    $"{prefix} . Commitment \"{commitmentTitle}\" is active and due for review.",

                _ => throw new Exception($"Unsupported HealthStatus value: {healthStatus}")
            };

            var memberIds = _membershipRepo.GetMembersByGroupId(groupId);
            foreach (var memberId in memberIds)
            {
                if (_notificationRepo.NotificationExists(memberId, commitmentId, healthStatus))
                    continue;

                _notificationRepo.CreateNotification(new Notification
                {
                    UserId = memberId,
                    GroupId = groupId,
                    CommitmentId = commitmentId,
                    HealthStatus = healthStatus,
                    Message = message,
                    IsRead = false
                });
            }
        }

        public List<NotificationResponse> GetNotificationsForUser(int userId)
        {
            return _notificationRepo.GetNotificationsByUserId(userId)
                .Select(n => new NotificationResponse
                {
                    NotificationId = n.NotificationId,
                    CommitmentId = n.CommitmentId,
                    GroupId = n.GroupId,
                    HealthStatus = n.HealthStatus,
                    Message = n.Message,
                    IsRead = n.IsRead,
                    CreatedAt = n.CreatedAt
                })
                .ToList();
        }

        public int GetUnreadCount(int userId)
        {
            return _notificationRepo.GetUnreadCount(userId);
        }

        public void MarkAsRead(int notificationId)
        {
            _notificationRepo.MarkAsRead(notificationId);
        }

        public NotificationResponse? GetByIdForUser(int notificationId, int userId)
        {
            var n = _notificationRepo.GetNotificationByIdForUser(notificationId, userId);
            if (n == null) return null;

            return new NotificationResponse
            {
                NotificationId = n.NotificationId,
                CommitmentId = n.CommitmentId,
                GroupId = n.GroupId,
                HealthStatus = n.HealthStatus,
                Message = n.Message,
                IsRead = n.IsRead,
                CreatedAt = n.CreatedAt
            };
        }

    }
}
