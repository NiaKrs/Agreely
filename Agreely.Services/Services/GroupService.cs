using Agreely.Repositories.Interfaces;
using Agreely.Services.DTO.Requests;
using Agreely.Services.DTO.Responses;
using Agreely.Services.Interfaces;
using Agreely.Domain;

namespace Agreely.Services.Services
{
    public class GroupService : IGroupService
    {
        private readonly IGroupRepository _groupRepo;
        private readonly IGroupMembershipRepository _membershipRepo;
        private readonly ICommitmentService _commitmentService;

        public GroupService(IGroupRepository groupRepo, IGroupMembershipRepository membershipRepo, ICommitmentService commitmentService)
        {
            _groupRepo = groupRepo;
            _membershipRepo = membershipRepo;
            _commitmentService = commitmentService;
        }

        public int CreateGroup(CreateGroupRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Name))
                throw new Exception("Group name is required.");

            var group = new Group
            {
                Name = request.Name,
                Description = request.Description,
                CreatedByUserId = request.CreatedByUserId
            };

            int groupId = _groupRepo.CreateGroup(group);

            var membership = new GroupMembership
            {
                GroupId = groupId,
                UserId = request.CreatedByUserId
            };

            _membershipRepo.AddMember(membership);

            return groupId;
        }

        public void JoinGroup(JoinGroupRequest request)
        {
            var group = _groupRepo.GetGroupById(request.GroupId);
            if (group == null)
                throw new Exception("Group not found.");

            if (_membershipRepo.IsMember(request.GroupId, request.UserId))
                throw new Exception("You are already a member of this group.");

            var membership = new GroupMembership
            {
                GroupId = request.GroupId,
                UserId = request.UserId
            };

            _membershipRepo.AddMember(membership);
        }

        public GroupDetailsResponse GetGroupDetails(int groupId)
        {
            var group = _groupRepo.GetGroupById(groupId);
            if (group == null)
                throw new Exception("Group not found.");

            var commitments = _commitmentService.GetCommitmentsByGroupId(groupId);
            int memberCount = _groupRepo.GetMemberCount(groupId);

            return new GroupDetailsResponse
            {
                GroupId = group.GroupId,
                Name = group.Name,
                Description = group.Description,
                MemberCount = memberCount,
                Commitments = commitments
            };
        }

        public List<GroupSummaryResponse> GetUserGroups(int userId)
        {
            var groups = _groupRepo.GetGroupsByUserId(userId);

            return groups.Select(g => new GroupSummaryResponse
            {
                GroupId = g.GroupId,
                Name = g.Name,
                Description = g.Description
            }).ToList();
        }

        public bool IsUserMember(int groupId, int userId)
        {
            return _membershipRepo.IsMember(groupId, userId);
        }
    }
}
