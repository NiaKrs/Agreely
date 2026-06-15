using Agreely.Domain.Enums;

namespace Agreely.Services.DTO.Responses
{
    public class NotificationResponse
    {
        public int NotificationId { get; set; }
        public int GroupId { get; set; }
        public int CommitmentId { get; set; }
        public HealthStatusValue HealthStatus { get; set; }
        public string Message { get; set; } = string.Empty;
        public bool IsRead { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
