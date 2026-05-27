using Agreely.Domain;
using Agreely.Domain.Enums;
using Agreely.Repositories.Entities;

namespace Agreely.Repositories.Mappers
{
    public static class CommitmentMapper
    {
        public static Commitment ToDomain(CommitmentEntity entity) => new Commitment
        {
            CommitmentId = entity.CommitmentId,
            GroupId = entity.GroupId,
            CreatedByUserId = entity.CreatedByUserId,
            CreatedAt = entity.CreatedAt,
            Status = (CommitmentStatus)entity.Status
        };
    }
}
