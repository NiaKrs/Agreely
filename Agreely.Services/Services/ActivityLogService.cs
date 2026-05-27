using Agreely.Domain;
using Agreely.Domain.Enums;
using Agreely.Services.DTO.Responses;
using Agreely.Services.Interfaces;  
using Agreely.Repositories.Interfaces;

namespace Agreely.Services.Services
{
    public class ActivityLogService : IActivityLogService
    {
        private readonly IActivityLogRepository _activityLogRepository;

        public ActivityLogService(IActivityLogRepository activityLogRepository)
        {
            _activityLogRepository = activityLogRepository;
        }

        public void LogEvent(int groupId, int userId, EventTypeValue eventType, string description)
        {
            var logEntry = new ActivityLog
            {
                GroupId = groupId,
                UserId = userId,
                EventType = eventType,
                OccuredAt = DateTime.Now,
                Description = description
            };
            _activityLogRepository.LogEvent(logEntry);
        }

        public List<ActivityLogResponse> GetGroupLog(int groupId)
        {
            var logEntries = _activityLogRepository.GetLogsByGroup(groupId);
            return logEntries.Select(log => new ActivityLogResponse
            {
                LogId = log.LogId,
                GroupId = log.GroupId,
                UserId = log.UserId,
                UserFullName = log.UserFullName,
                EventType = log.EventType,
                OccuredAt = log.OccuredAt,
                Description = log.Description
            }).ToList();
        }
    }
}
