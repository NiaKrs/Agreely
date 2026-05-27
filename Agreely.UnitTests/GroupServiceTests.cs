using Agreely.Domain;
using Agreely.Repositories.Interfaces;
using Agreely.Services.DTO.Requests;
using Agreely.Services.DTO.Responses;
using Agreely.Services.Interfaces;
using Agreely.Services.Services;
using Moq;

namespace Agreely.UnitTests
{
    public class GroupServiceTests
    {
        private readonly Mock<IGroupRepository> _groupRepoMock;
        private readonly Mock<IGroupMembershipRepository> _membershipRepoMock;
        private readonly Mock<ICommitmentService> _commitmentServiceMock;
        private readonly GroupService _groupService;

        public GroupServiceTests()
        {
            _groupRepoMock = new Mock<IGroupRepository>();
            _membershipRepoMock = new Mock<IGroupMembershipRepository>();
            _commitmentServiceMock = new Mock<ICommitmentService>();
            _groupService = new GroupService(
                _groupRepoMock.Object,
                _membershipRepoMock.Object,
                _commitmentServiceMock.Object
            );
        }

        
        [Fact]
        public void CreateGroup_ValidRequest_CreatesGroupAndAddsMember()
        {
            var request = new CreateGroupRequest { Name = "Team", Description = "Desc", CreatedByUserId = 1 };
            _groupRepoMock.Setup(r => r.CreateGroup(It.IsAny<Group>())).Returns(5);

            var result = _groupService.CreateGroup(request);

            Assert.Equal(5, result);
            _membershipRepoMock.Verify(r => r.AddMember(It.Is<GroupMembership>(m => m.GroupId == 5 && m.UserId == 1)), Times.Once);
        }

        [Fact]
        public void JoinGroup_ValidRequest_AddsMember()
        {
            var request = new JoinGroupRequest { GroupId = 1, UserId = 2 };
            _groupRepoMock.Setup(r => r.GetGroupById(1)).Returns(new Group { GroupId = 1, Name = "Team" });
            _membershipRepoMock.Setup(r => r.IsMember(1, 2)).Returns(false);

            _groupService.JoinGroup(request);

            _membershipRepoMock.Verify(r => r.AddMember(It.IsAny<GroupMembership>()), Times.Once);
        }

        [Fact]
        public void JoinGroup_GroupNotFound_ThrowsException()
        {
            var request = new JoinGroupRequest { GroupId = 99, UserId = 1 };
            _groupRepoMock.Setup(r => r.GetGroupById(99)).Returns((Group?)null);

            var ex = Assert.Throws<Exception>(() => _groupService.JoinGroup(request));
            Assert.Equal("Group not found.", ex.Message);
        }

        [Fact]
        public void JoinGroup_AlreadyMember_ThrowsException()
        {
            var request = new JoinGroupRequest { GroupId = 1, UserId = 1 };
            _groupRepoMock.Setup(r => r.GetGroupById(1)).Returns(new Group { GroupId = 1, Name = "Team" });
            _membershipRepoMock.Setup(r => r.IsMember(1, 1)).Returns(true);

            var ex = Assert.Throws<Exception>(() => _groupService.JoinGroup(request));
            Assert.Equal("You are already a member of this group.", ex.Message);
        }

        [Fact]
        public void GetGroupDetails_ExistingGroup_ReturnsDetails()
        {
            _groupRepoMock.Setup(r => r.GetGroupById(1)).Returns(new Group { GroupId = 1, Name = "Team", Description = "Desc" });
            _groupRepoMock.Setup(r => r.GetMemberCount(1)).Returns(4);
            _commitmentServiceMock.Setup(r => r.GetCommitmentsByGroupId(1)).Returns(new List<ViewCommitmentResponse>());

            var result = _groupService.GetGroupDetails(1);

            Assert.Equal("Team", result.Name);
            Assert.Equal(4, result.MemberCount);
        }

        [Fact]
        public void GetGroupDetails_GroupNotFound_ThrowsException()
        {
            _groupRepoMock.Setup(r => r.GetGroupById(99)).Returns((Group?)null);

            var ex = Assert.Throws<Exception>(() => _groupService.GetGroupDetails(99));
            Assert.Equal("Group not found.", ex.Message);
        }

        [Fact]
        public void GetUserGroups_ValidUserId_ReturnsMappedList()
        {
            _groupRepoMock.Setup(r => r.GetGroupsByUserId(1)).Returns(new List<Group>
            {
                new Group { GroupId = 1, Name = "A" },
                new Group { GroupId = 2, Name = "B" }
            });

            var result = _groupService.GetUserGroups(1);

            Assert.Equal(2, result.Count);
        }
    }
}