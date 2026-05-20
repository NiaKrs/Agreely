

using Agreely.Domain;

namespace Agreely.Repositories.Interfaces
{
    public interface IActivityLogRepository
    {
        void LogEvent(ActivityLog entry);
        List<ActivityLog> GetLogsByGroup(int groupId);
    }
}
