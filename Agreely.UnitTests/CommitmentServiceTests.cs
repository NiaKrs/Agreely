using Agreely.Domain;
using Agreely.Domain.Enums;
using Agreely.Repositories.Interfaces;
using Agreely.Services.DTO.Requests;
using Agreely.Services.Interfaces;
using Agreely.Services.Services;
using Agreely.Services;
using Microsoft.Extensions.Configuration;
using Moq;

namespace Agreely.UnitTests
{
    public class CommitmentServiceTests
    {
        private readonly Mock<ICommitmentRepository> _commitmentRepoMock;
        private readonly Mock<IGroupMembershipRepository> _membershipRepoMock;
        private readonly Mock<IActivityLogService> _activityLogServiceMock;
        private readonly Mock<HealthStatusEvaluator> _healthStatusEvaluatorMock;
        private readonly CommitmentService _commitmentService;

        public CommitmentServiceTests()
        {
            _commitmentRepoMock = new Mock<ICommitmentRepository>();
            _membershipRepoMock = new Mock<IGroupMembershipRepository>();
            _activityLogServiceMock = new Mock<IActivityLogService>();
            _healthStatusEvaluatorMock = new Mock<HealthStatusEvaluator>(new Mock<IConfiguration>().Object);
            _commitmentService = new CommitmentService(
                _commitmentRepoMock.Object,
                _membershipRepoMock.Object,
                _activityLogServiceMock.Object,
                _healthStatusEvaluatorMock.Object
            );
        }

        [Fact]
        public void CreateCommitment_ValidRequest_ReturnsCommitmentId()
        {
            var request = new CreateCommitmentRequest { GroupId = 1, CreatedByUserId = 1, Title = "Test", Description = "Desc" };
            _membershipRepoMock.Setup(r => r.IsMember(1, 1)).Returns(true);
            _commitmentRepoMock.Setup(r => r.InsertCommitment(It.IsAny<Commitment>())).Returns(10);
            _commitmentRepoMock.Setup(r => r.CreateCommitmentVersion(It.IsAny<CommitmentVersion>())).Returns(1);

            var result = _commitmentService.CreateCommitment(request);

            Assert.Equal(10, result);
            _activityLogServiceMock.Verify(a => a.LogEvent(1, 1, EventTypeValue.CommitmentCreated, It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public void CreateCommitment_UserNotMember_ThrowsException()
        {
            var request = new CreateCommitmentRequest { GroupId = 1, CreatedByUserId = 2, Title = "Test" };
            _membershipRepoMock.Setup(r => r.IsMember(1, 2)).Returns(false);

            var ex = Assert.Throws<Exception>(() => _commitmentService.CreateCommitment(request));
            Assert.Equal("You must be a member of the group to create a commitment.", ex.Message);
        }

        [Fact]
        public void GetCommitmentsByGroupId_ValidGroupId_ReturnsList()
        {
            _commitmentRepoMock.Setup(r => r.GetCommitmentsByGroupId(1)).Returns(new List<Commitment>
            {
                new Commitment { CommitmentId = 1 },
                new Commitment { CommitmentId = 2 }
            });
            _commitmentRepoMock.Setup(r => r.GetCurrentVersion(It.IsAny<int>()))
                .Returns(new CommitmentVersion { Title = "Test", IsActive = true });

            var result = _commitmentService.GetCommitmentsByGroupId(1);

            Assert.Equal(2, result.Count);
        }

        [Fact]
        public void GetCommitmentById_ExistingId_ReturnsCommitment()
        {
            _commitmentRepoMock.Setup(r => r.GetCommitmentById(1)).Returns(new Commitment { CommitmentId = 1 });
            _commitmentRepoMock.Setup(r => r.GetCurrentVersion(1)).Returns(new CommitmentVersion { Title = "Test", IsActive = true });

            var result = _commitmentService.GetCommitmentById(1);

            Assert.NotNull(result);
            Assert.Equal(1, result.CommitmentId);
        }

        [Fact]
        public void GetCommitmentById_NonExistingId_ReturnsNull()
        {
            _commitmentRepoMock.Setup(r => r.GetCommitmentById(99)).Returns((Commitment?)null);

            var result = _commitmentService.GetCommitmentById(99);

            Assert.Null(result);
        }

        [Fact]
        public void CreateCommitmentVersion_ValidRequest_DeactivatesPreviousAndReturnsNewId()
        {
            var request = new CreateCommitmentVersionRequest { CommitmentId = 1, Title = "Updated", Description = "New", CreatedByUserId = 1 };
            _commitmentRepoMock.Setup(r => r.GetCommitmentById(1)).Returns(new Commitment { CommitmentId = 1, GroupId = 1 });
            _commitmentRepoMock.Setup(r => r.GetCurrentVersion(1)).Returns(new CommitmentVersion { Id = 1, IsActive = true });
            _commitmentRepoMock.Setup(r => r.CreateCommitmentVersion(It.IsAny<CommitmentVersion>())).Returns(2);

            var result = _commitmentService.CreateCommitmentVersion(request);

            Assert.Equal(2, result);
            _commitmentRepoMock.Verify(r => r.DeactivatePreviousVersions(1), Times.Once);
            _activityLogServiceMock.Verify(a => a.LogEvent(1, 1, EventTypeValue.CommitmentRevised, It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public void CreateCommitmentVersion_CommitmentNotFound_ThrowsException()
        {
            var request = new CreateCommitmentVersionRequest { CommitmentId = 99, Title = "Updated" };
            _commitmentRepoMock.Setup(r => r.GetCommitmentById(99)).Returns((Commitment?)null);

            var ex = Assert.Throws<Exception>(() => _commitmentService.CreateCommitmentVersion(request));
            Assert.Equal("Commitment not found.", ex.Message);
        }

        [Fact]
        public void DeleteCommitment_ExistingId_CallsRepositoryDelete()
        {
            _commitmentRepoMock.Setup(r => r.GetCommitmentById(1)).Returns(new Commitment { CommitmentId = 1 });

            _commitmentService.DeleteCommitment(1);

            _commitmentRepoMock.Verify(r => r.DeleteVotesByCommitmentId(1), Times.Once);
            _commitmentRepoMock.Verify(r => r.DeleteVersionsByCommitmentId(1), Times.Once);
            _commitmentRepoMock.Verify(r => r.DeleteCommitment(1), Times.Once);
        }
        

        [Fact]
        public void DeleteCommitment_NonExistingId_ThrowsException()
        {
            _commitmentRepoMock.Setup(r => r.GetCommitmentById(99)).Returns((Commitment?)null);

            var ex = Assert.Throws<Exception>(() => _commitmentService.DeleteCommitment(99));
            Assert.Equal("Commitment not found.", ex.Message);
        }

        [Fact]
        public void CreateCommitment_EmptyTitle_ThrowsException()
        {
            var request = new CreateCommitmentRequest { GroupId = 1, CreatedByUserId = 1, Title = "" };

            var ex = Assert.Throws<Exception>(() => _commitmentService.CreateCommitment(request));
            Assert.Equal("Commitment title is required.", ex.Message);
        }

        [Fact]
        public void CreateCommitmentVersion_EmptyTitle_ThrowsException()
        {
            var request = new CreateCommitmentVersionRequest { CommitmentId = 1, Title = "", CreatedByUserId = 1 };

            var ex = Assert.Throws<Exception>(() => _commitmentService.CreateCommitmentVersion(request));
            Assert.Equal("Commitment title is required.", ex.Message);
        }
    }
}