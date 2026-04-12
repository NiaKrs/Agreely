using Agreely.Repositories.Models;

namespace Agreely.Repositories.Interfaces
{
    public interface IGroupMembershipRepository
    {
        void AddMember(GroupMembership membership);
        
    }
}
