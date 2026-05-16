using Agreely.Services.DTO.Requests;
using Agreely.Services.DTO.Responses;

namespace Agreely.Services.Interfaces
{
    public interface IGroupService
    {
        int CreateGroup(CreateGroupRequest dto);
        void JoinGroup(JoinGroupRequest dto);
        GroupDetailsResponse GetGroupDetails(int groupId);
        List<GroupSummaryResponse> GetUserGroups(int userId);
    }
}