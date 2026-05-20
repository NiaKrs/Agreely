

using Agreely.Domain.Enums;
using Agreely.Services.DTO.Responses;

namespace Agreely.Services.Interfaces
{
    public interface IActivityLogService
    {
        void LogEvent(int groupId, int userId, EventTypeValue eventType, string description);
        List<ActivityLogResponse> GetGroupLog(int groupId);

    }
}
