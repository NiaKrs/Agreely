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
        private readonly HealthStatusEvaluator _healthStatusEvaluator;
        private readonly CommitmentService _commitmentService;

        public CommitmentServiceTests()
        {
            _commitmentRepoMock = new Mock<ICommitmentRepository>();
            _membershipRepoMock = new Mock<IGroupMembershipRepository>();
            _activityLogServiceMock = new Mock<IActivityLogService>();

            var config = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string?>
                {
            { "CommitmentHealth:PendingStaleAfterDays", "7" },
            { "CommitmentHealth:ReviewDueAfterDays", "30" }
                })
                .Build();
            _healthStatusEvaluator = new HealthStatusEvaluator(config);

            _commitmentService = new CommitmentService(
                _commitmentRepoMock.Object,
                _membershipRepoMock.Object,
                _activityLogServiceMock.Object,
                _healthStatusEvaluator
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
        public void DeleteCommitment_ExistingId_CallsAllDeleteStepsInOrder()
        {
            _commitmentRepoMock.Setup(r => r.GetCommitmentById(1)).Returns(new Commitment { CommitmentId = 1 });

            _commitmentService.DeleteCommitment(1);

            _commitmentRepoMock.Verify(r => r.DeleteVotesByCommitmentId(1), Times.Once);
            _commitmentRepoMock.Verify(r => r.DeleteNotificationsByCommitmentId(1), Times.Once);
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

        [Fact]
        public void GetCommitmentsByGroupId_VersionMissing_ReturnsHealthyAsDefault()
        {
            _commitmentRepoMock.Setup(r => r.GetCommitmentsByGroupId(1)).Returns(new List<Commitment>
            {
                new Commitment { CommitmentId = 1, Status = CommitmentStatus.Pending }
            });
            // No active version found
            _commitmentRepoMock.Setup(r => r.GetCurrentVersion(1)).Returns((CommitmentVersion?)null);

            var result = _commitmentService.GetCommitmentsByGroupId(1);

            Assert.Equal(HealthStatusValue.Healthy, result[0].HealthStatus);
        }

        [Fact]
        public void GetCommitmentById_PendingAndStale_ReturnsNeedsAttention()
        {
            _commitmentRepoMock.Setup(r => r.GetCommitmentById(1)).Returns(new Commitment
            {
                CommitmentId = 1,
                Status = CommitmentStatus.Pending
            });
            _commitmentRepoMock.Setup(r => r.GetCurrentVersion(1)).Returns(new CommitmentVersion
            {
                Title = "Test",
                IsActive = true,
                // 8 days old — over the 7-day pending threshold
                CreatedAt = DateTime.Now.AddDays(-8)
            });

            var result = _commitmentService.GetCommitmentById(1);

            Assert.Equal(HealthStatusValue.NeedsAttention, result!.HealthStatus);
        }

        [Fact]
        public void GetCommitmentById_ActiveAndOld_ReturnsDueForReview()
        {
            _commitmentRepoMock.Setup(r => r.GetCommitmentById(1)).Returns(new Commitment
            {
                CommitmentId = 1,
                Status = CommitmentStatus.Active
            });
            _commitmentRepoMock.Setup(r => r.GetCurrentVersion(1)).Returns(new CommitmentVersion
            {
                Title = "Test",
                IsActive = true,
                // 31 days old — over the 30-day review threshold
                CreatedAt = DateTime.Now.AddDays(-31)
            });

            var result = _commitmentService.GetCommitmentById(1);

            Assert.Equal(HealthStatusValue.DueForReview, result!.HealthStatus);
        }

        [Fact]
        public void GetCommitmentById_PendingAndFresh_ReturnsHealthy()
        {
            _commitmentRepoMock.Setup(r => r.GetCommitmentById(1)).Returns(new Commitment
            {
                CommitmentId = 1,
                Status = CommitmentStatus.Pending
            });
            _commitmentRepoMock.Setup(r => r.GetCurrentVersion(1)).Returns(new CommitmentVersion
            {
                Title = "Test",
                IsActive = true,
                // 2 days old — within threshold
                CreatedAt = DateTime.Now.AddDays(-2)
            });

            var result = _commitmentService.GetCommitmentById(1);

            Assert.Equal(HealthStatusValue.Healthy, result!.HealthStatus);
        }

        [Fact]
        public void GetCommitmentById_ActiveAndFresh_ReturnsHealthy()
        {
            _commitmentRepoMock.Setup(r => r.GetCommitmentById(1)).Returns(new Commitment
            {
                CommitmentId = 1,
                Status = CommitmentStatus.Active
            });
            _commitmentRepoMock.Setup(r => r.GetCurrentVersion(1)).Returns(new CommitmentVersion
            {
                Title = "Test",
                IsActive = true,
                // 10 days old — within 30-day review threshold
                CreatedAt = DateTime.Now.AddDays(-10)
            });

            var result = _commitmentService.GetCommitmentById(1);

            Assert.Equal(HealthStatusValue.Healthy, result!.HealthStatus);
        }

        [Fact]
        public void RequestReview_ValidCommitment_DeletesVotesAndSetsPending()
        {
            _commitmentRepoMock.Setup(r => r.GetCommitmentById(1)).Returns(new Commitment { CommitmentId = 1, GroupId = 1 });
            _commitmentRepoMock.Setup(r => r.GetCurrentVersion(1)).Returns(new CommitmentVersion { Id = 1, Title = "Test", IsActive = true });

            _commitmentService.RequestReview(1, 2);

            _commitmentRepoMock.Verify(r => r.DeleteVotesByCommitmentId(1), Times.Once);
            _commitmentRepoMock.Verify(r => r.UpdateCommitmentStatus(1, CommitmentStatus.Pending), Times.Once);
            _activityLogServiceMock.Verify(a => a.LogEvent(1, 2, EventTypeValue.ReviewRequested, It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public void RequestReview_CommitmentNotFound_ThrowsException()
        {
            _commitmentRepoMock.Setup(r => r.GetCommitmentById(99)).Returns((Commitment?)null);

            var ex = Assert.Throws<Exception>(() => _commitmentService.RequestReview(99, 1));
            Assert.Equal("Commitment not found.", ex.Message);
        }

        [Fact]
        public void RequestReview_NoActiveVersion_ThrowsException()
        {
            _commitmentRepoMock.Setup(r => r.GetCommitmentById(1)).Returns(new Commitment { CommitmentId = 1 });
            _commitmentRepoMock.Setup(r => r.GetCurrentVersion(1)).Returns((CommitmentVersion?)null);

            var ex = Assert.Throws<Exception>(() => _commitmentService.RequestReview(1, 1));
            Assert.Equal("No active version found for this commitment.", ex.Message);
        }
    }
}