using Agreely.Domain;
using Agreely.Repositories.Entities;

namespace Agreely.Repositories.Mappers
{
    public static class GroupMapper
    {
        public static Group ToDomain(GroupEntity entity) => new Group
        {
            GroupId = entity.GroupId,
            Name = entity.Name,
            Description = entity.Description,
            CreatedAt = entity.CreatedAt,
            CreatedByUserId = entity.CreatedByUserId
        };
    }
}