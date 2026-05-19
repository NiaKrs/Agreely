using Agreely.Domain;
using Agreely.Repositories.Entities;

namespace Agreely.Repositories.Mappers
{
    public static class CommitmentVersionMapper
    {
        public static CommitmentVersion ToDomain(CommitmentVersionEntity entity) => new CommitmentVersion
        {
            Id = entity.Id,
            CommitmentId = entity.CommitmentId,
            CreatedByUserId = entity.CreatedByUserId,
            Title = entity.Title,
            Description = entity.Description,
            CreatedAt = entity.CreatedAt,
            IsActive = entity.IsActive
        };
    }
}
