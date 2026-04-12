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

        public GroupService(IGroupRepository groupRepo, IGroupMembershipRepository membershipRepo)
        {
            _groupRepo = groupRepo;
            _membershipRepo = membershipRepo;
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
    }
}
