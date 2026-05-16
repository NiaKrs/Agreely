using Agreely.Domain;
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
            Title = entity.Title,
            Description = entity.Description,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt
        };
    }
}
