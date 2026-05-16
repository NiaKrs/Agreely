using Agreely.Domain;
using Agreely.Repositories.Entities;

namespace Agreely.Repositories.Mappers
{
    public static class GroupMembershipMapper
    {
        public static GroupMembership ToDomain(GroupMembershipEntity entity) => new GroupMembership
        {
            GroupMembershipId = entity.GroupMembershipId,
            GroupId = entity.GroupId,
            UserId = entity.UserId,
            JoinedAt = entity.JoinedAt
        };
    }
}