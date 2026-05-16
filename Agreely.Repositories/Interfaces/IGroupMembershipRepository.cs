using Agreely.Domain;

namespace Agreely.Repositories.Interfaces
{
    public interface IGroupMembershipRepository
    {
        void AddMember(GroupMembership membership);
        bool IsMember(int groupId, int userId);

    }
}
