using Agreely.Repositories.Interfaces;
using Agreely.Repositories.Models;
using Agreely.Services.DTO;
using Agreely.Services.Interfaces;

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

        public int CreateGroup(CreateGroupDto dto)
        {
            var group = new Group
            {
                Name = dto.Name,
                Description = dto.Description,
                CreatedByUserId = dto.CreatedByUserId
            };

            int groupId = _groupRepo.CreateGroup(group);

            var membership = new GroupMembership
            {
                GroupId = groupId,
                UserId = dto.CreatedByUserId
            };

            _membershipRepo.AddMember(membership);

            return groupId;
        }

        public void JoinGroup(JoinGroupDto dto)
        {
            var group = _groupRepo.GetGroupById(dto.GroupId);
            if (group == null)
                throw new Exception("Group not found.");

            if (_membershipRepo.IsMember(dto.GroupId, dto.UserId))
                throw new Exception("You are already a member of this group.");

            var membership = new GroupMembership
            {
                GroupId = dto.GroupId,
                UserId = dto.UserId
            };

            _membershipRepo.AddMember(membership);
        }

        public GroupDetailsDto GetGroupDetails(int groupId)
        {
            var group = _groupRepo.GetGroupById(groupId);
            if (group == null)
                throw new Exception("Group not found.");

            var commitments = _commitmentService.GetCommitmentsByGroupId(groupId);
            int memberCount = _groupRepo.GetMemberCount(groupId);

            return new GroupDetailsDto
            {
                GroupId = group.GroupId,
                Name = group.Name,
                Description = group.Description,
                MemberCount = memberCount,
                Commitments = commitments.Select(c => new ViewCommitmentDto
                {
                    CommitmentId = c.CommitmentId,
                    Title = c.Title,
                    Description = c.Description
                }).ToList()
            };
        }
    }
}
