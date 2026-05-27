using Agreely.Domain;
using Agreely.Domain.Enums;
using Agreely.Repositories.Entities;

namespace Agreely.Repositories.Mappers
{
    public static class AlignmentVoteMapper
    {
        public static AlignmentVote ToDomain(AlignmentVoteEntity entity) => new AlignmentVote
        {
            VoteId = entity.VoteId,
            CommitmentVersionId = entity.CommitmentVersionId,
            UserId = entity.UserId,
            Vote = (VoteValue)entity.Vote,
            CreatedAt = entity.CreatedAt
        };
    }
}
