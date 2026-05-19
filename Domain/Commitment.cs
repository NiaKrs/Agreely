using Agreely.Domain.Enums;

namespace Agreely.Domain
{
    public class Commitment
    {
        public int CommitmentId { get; set; }
        public int GroupId { get; set; }
        public int CreatedByUserId { get; set; }
        public DateTime CreatedAt { get; set; }
        public CommitmentStatus Status { get; set; }

        
    }
}