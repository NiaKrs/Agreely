using Agreely.Domain.Enums;

namespace Agreely.Domain
{
    public class Notification
    {
        public int NotificationId { get; set; }
        public int UserId { get; set; }
        public int GroupId { get; set; }
        public int CommitmentId { get; set; }
        public HealthStatusValue HealthStatus { get; set; }
        public string Message { get; set; } = string.Empty;
        public bool IsRead { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
