
using Agreely.Domain;
using Agreely.Domain.Enums;
using Agreely.Repositories.Entities;

namespace Agreely.Repositories.Mappers
{
    public static class ActivityLogMapper
    {
        public static ActivityLog ToDomain(ActivityLogEntity entity) => new ActivityLog
        {
            LogId = entity.LogId,
            GroupId = entity.GroupId,
            UserId = entity.UserId,
            EventType = (EventTypeValue)entity.EventType,
            OccuredAt = entity.OccuredAt,
            Description = entity.Description,
            UserFullName = entity.UserFullName
        };
    }
}
