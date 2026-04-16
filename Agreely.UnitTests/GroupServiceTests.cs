using System.Timers;
using Agreely.Repositories.Interfaces;
using Agreely.Repositories.Models;
using Agreely.Services.DTO;
using Agreely.Services.Interfaces;
using Agreely.Services.Services;
using Moq;

namespace Agreely.Tests
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

        // TC-01-01: CreateGroup success
        [Fact]
        public void CreateGroup_ValidDto_ReturnsGroupId()
        {
            var dto = new CreateGroupDto { Name = "Test Group", Description = "Desc", CreatedByUserId = 1 };
            _groupRepoMock.Setup(r => r.CreateGroup(It.IsAny<Group>())).Returns(42);

            var result = _groupService.CreateGroup(dto);

            Assert.Equal(42, result);
            _membershipRepoMock.Verify(r => r.AddMember(It.IsAny<GroupMembership>()), Times.Once);
        }

        // TC-02-01: JoinGroup success
        [Fact]
        public void JoinGroup_ValidDto_AddsMember()
        {
            var dto = new JoinGroupDto { GroupId = 1, UserId = 2 };
            _groupRepoMock.Setup(r => r.GetGroupById(1)).Returns(new Group { GroupId = 1, Name = "Test" });
            _membershipRepoMock.Setup(r => r.IsMember(1, 2)).Returns(false);

            _groupService.JoinGroup(dto);

            _membershipRepoMock.Verify(r => r.AddMember(It.IsAny<GroupMembership>()), Times.Once);
        }

        // TC-02-02: JoinGroup group not found
        [Fact]
        public void JoinGroup_GroupNotFound_ThrowsException()
        {
            var dto = new JoinGroupDto { GroupId = 99, UserId = 1 };
            _groupRepoMock.Setup(r => r.GetGroupById(99)).Returns((Group?)null);

            var ex = Assert.Throws<Exception>(() => _groupService.JoinGroup(dto));
            Assert.Equal("Group not found.", ex.Message);
        }

        // TC-02-03: JoinGroup already a member
        [Fact]
        public void JoinGroup_AlreadyMember_ThrowsException()
        {
            var dto = new JoinGroupDto { GroupId = 1, UserId = 1 };
            _groupRepoMock.Setup(r => r.GetGroupById(1)).Returns(new Group { GroupId = 1, Name = "Test" });
            _membershipRepoMock.Setup(r => r.IsMember(1, 1)).Returns(true);

            var ex = Assert.Throws<Exception>(() => _groupService.JoinGroup(dto));
            Assert.Equal("You are already a member of this group.", ex.Message);
        }

        // TC-03-01: GetGroupDetails success
        [Fact]
        public void GetGroupDetails_ValidGroupId_ReturnsDetails()
        {
            _groupRepoMock.Setup(r => r.GetGroupById(1)).Returns(new Group { GroupId = 1, Name = "Test" });
            _groupRepoMock.Setup(r => r.GetMemberCount(1)).Returns(3);
            _commitmentServiceMock.Setup(s => s.GetCommitmentsByGroupId(1)).Returns(new List<Commitment>());

            var result = _groupService.GetGroupDetails(1);

            Assert.Equal(1, result.GroupId);
            Assert.Equal("Test", result.Name);
            Assert.Equal(3, result.MemberCount);
        }

        // GetGroupDetails group not found
        [Fact]
        public void GetGroupDetails_GroupNotFound_ThrowsException()
        {
            _groupRepoMock.Setup(r => r.GetGroupById(99)).Returns((Group?)null);

            var ex = Assert.Throws<Exception>(() => _groupService.GetGroupDetails(99));
            Assert.Equal("Group not found.", ex.Message);
        }

        // TC-03-01: GetUserGroups returns list
        [Fact]
        public void GetUserGroups_ValidUserId_ReturnsList()
        {
            _groupRepoMock.Setup(r => r.GetGroupsByUserId(1)).Returns(new List<Group>
            {
                new Group { GroupId = 1, Name = "Group A" },
                new Group { GroupId = 2, Name = "Group B" }
            });

            var result = _groupService.GetUserGroups(1);

            Assert.Equal(2, result.Count);
        }
    }
}