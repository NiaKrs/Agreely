using Agreely.Domain.Enums;

namespace Agreely.Repositories.Entities
{
    public class CommitmentEntity
    {
        public int CommitmentId { get; set; }
        public int GroupId { get; set; }
        public int CreatedByUserId { get; set; }
        public DateTime CreatedAt { get; set; }
        public int Status { get; set; }
    }
}