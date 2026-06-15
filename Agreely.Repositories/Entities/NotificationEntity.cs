

namespace Agreely.Repositories.Entities
{
    public class NotificationEntity
    {
        public int NotificationId { get; set; }
        public int UserId { get; set; }
        public int GroupId { get; set; }
        public int CommitmentId { get; set; }
        public int HealthStatus { get; set; }
        public string Message { get; set; } = string.Empty;
        public bool IsRead { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
