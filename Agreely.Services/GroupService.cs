using Agreely.Repositories;
using Agreely.Models;

namespace Agreely.Services
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

        public int CreateGroup(string name, string? description, int userId)
        {
            var group = new Group
            {
                Name = name,
                Description = description,
                CreatedByUserId = userId
            };

            int groupId = _groupRepo.CreateGroup(group);

            var membership = new GroupMembership
            {
                GroupId = groupId,
                UserId = userId,
            };

            _membershipRepo.AddMember(membership);

            return groupId;
        }
    }
}
