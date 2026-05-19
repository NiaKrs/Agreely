

namespace Agreely.Repositories.Entities
{
    public class AlignmentVoteEntity
    {
        public int VoteId { get; set; }
        public int CommitmentVersionId { get; set; }
        public int UserId { get; set; }
        public int Vote { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
