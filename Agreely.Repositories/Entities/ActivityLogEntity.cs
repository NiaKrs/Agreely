

namespace Agreely.Repositories.Entities
{
    public class ActivityLogEntity
    {
        public int LogId { get; set; }
        public int GroupId { get; set; }
        public int UserId { get; set; }
        public int EventType { get; set; } 
        public DateTime OccuredAt { get; set; }
        public string Description { get; set; } = string.Empty;
        public string UserFullName { get; set; } = string.Empty;
    }
}
