using Agreely.Domain.Enums;

namespace Agreely.Domain
{
    public class AlignmentVote
    {
        public int VoteId { get; set; }
        public int CommitmentVersionId { get; set; }
        public int UserId { get; set; }
        public VoteValue Vote { get; set; }
        public DateTime CreatedAt { get; set; }

        
    }
}
