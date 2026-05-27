using Agreely.Domain.Enums;

namespace Agreely.Domain
{
    public class ActivityLog
    {
        public int LogId { get; set; }
        public int GroupId { get; set; }
        public int UserId { get; set; }
        public EventTypeValue EventType { get; set; }
        public DateTime OccuredAt { get; set; }
        public string Description { get; set; } = string.Empty;
        public string UserFullName { get; set; } = string.Empty;
    }
}
