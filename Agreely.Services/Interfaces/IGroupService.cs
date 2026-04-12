using Agreely.Services.DTO;

namespace Agreely.Services.Interfaces
{
    public interface IGroupService
    {
        int CreateGroup(CreateGroupDto dto);
        void JoinGroup(JoinGroupDto dto);
    }
}