using Agreely.Repositories.Models;

namespace Agreely.Repositories.Interfaces
{
    public interface IGroupRepository
    {
        int CreateGroup(Group group);
        Group GetGroupById(int groupId);
    }
}
