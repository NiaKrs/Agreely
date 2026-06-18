using Agreely.Domain;
using Agreely.Domain.Enums;
using Agreely.Repositories.Entities;


namespace Agreely.Repositories.Mappers
{
    public static class NotificationMapper
    {
        public static Notification ToDomain(NotificationEntity entity)
        {
            return new Notification
            {
                NotificationId = entity.NotificationId,
                UserId = entity.UserId,
                GroupId = entity.GroupId,
                CommitmentId = entity.CommitmentId,
                HealthStatus = (HealthStatusValue)entity.HealthStatus,
                Message = entity.Message,
                IsRead = entity.IsRead,
                CreatedAt = entity.CreatedAt
            };
        }
    }
}
