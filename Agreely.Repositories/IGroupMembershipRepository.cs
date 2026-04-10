using Agreely.Models;

namespace Agreely.Repositories
{
    public interface IGroupMembershipRepository
    {
        void AddMember(GroupMembership membership);
        
    }
}
