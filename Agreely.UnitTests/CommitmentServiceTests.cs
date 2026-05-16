using Agreely.Repositories.Interfaces;
using Agreely.Services.DTO.Requests;
using Agreely.Domain;
using Agreely.Services.Services;
using Moq;

namespace Agreely.Tests
{
    public class CommitmentServiceTests
    {
        private readonly Mock<ICommitmentRepository> _commitmentRepoMock;
        private readonly Mock<IGroupMembershipRepository> _membershipRepoMock;
        private readonly CommitmentService _commitmentService;

        public CommitmentServiceTests()
        {
            _commitmentRepoMock = new Mock<ICommitmentRepository>();
            _membershipRepoMock = new Mock<IGroupMembershipRepository>();
            _commitmentService = new CommitmentService(
                _commitmentRepoMock.Object,
                _membershipRepoMock.Object
            );
        }

        // TC-04-01: CreateCommitment success
        [Fact]
        public void CreateCommitment_ValidDto_ReturnsCommitmentId()
        {
            var dto = new CreateCommitmentRequest { GroupId = 1, CreatedByUserId = 1, Title = "Test", Description = "Desc" };
            _membershipRepoMock.Setup(r => r.IsMember(1, 1)).Returns(true);
            _commitmentRepoMock.Setup(r => r.CreateCommitment(It.IsAny<Commitment>())).Returns(10);

            var result = _commitmentService.CreateCommitment(dto);

            Assert.Equal(10, result);
        }

        // CreateCommitment not a member
        [Fact]
        public void CreateCommitment_NotMember_ThrowsException()
        {
            var dto = new CreateCommitmentRequest { GroupId = 1, CreatedByUserId = 2, Title = "Test" };
            _membershipRepoMock.Setup(r => r.IsMember(1, 2)).Returns(false);

            var ex = Assert.Throws<Exception>(() => _commitmentService.CreateCommitment(dto));
            Assert.Equal("You must be a member of the group to create a commitment.", ex.Message);
        }

        // TC-05-01: GetCommitmentsByGroupId success
        [Fact]
        public void GetCommitmentsByGroupId_ValidGroupId_ReturnsList()
        {
            _commitmentRepoMock.Setup(r => r.GetCommitmentsByGroupId(1)).Returns(new List<Commitment>
            {
                new Commitment { CommitmentId = 1, Title = "Commit A" },
                new Commitment { CommitmentId = 2, Title = "Commit B" }
            });

            var result = _commitmentService.GetCommitmentsByGroupId(1);

            Assert.Equal(2, result.Count);
        }

        // GetCommitmentById - found
        [Fact]
        public void GetCommitmentById_ValidId_ReturnsCommitment()
        {
            // Arrange
            _commitmentRepoMock.Setup(r => r.GetCommitmentById(1))
                .Returns(new Commitment { CommitmentId = 1, Title = "Test" });

            // Act
            var result = _commitmentService.GetCommitmentById(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.CommitmentId);
        }

        // GetCommitmentById - not found
        [Fact]
        public void GetCommitmentById_InvalidId_ReturnsNull()
        {
            // Arrange
            _commitmentRepoMock.Setup(r => r.GetCommitmentById(99))
                .Returns((Commitment?)null);

            // Act
            var result = _commitmentService.GetCommitmentById(99);

            // Assert
            Assert.Null(result);
        }

        // TC-06-01: UpdateCommitment success
        [Fact]
        public void UpdateCommitment_ValidDto_UpdatesCommitment()
        {
            var dto = new UpdateCommitmentRequest { CommitmentId = 1, Title = "Updated", Description = "New Desc" };
            _commitmentRepoMock.Setup(r => r.GetCommitmentById(1)).Returns(new Commitment { CommitmentId = 1, Title = "Old" });

            _commitmentService.UpdateCommitment(dto);

            _commitmentRepoMock.Verify(r => r.UpdateCommitment(It.Is<Commitment>(c => c.Title == "Updated")), Times.Once);
        }

        // TC-06-01: UpdateCommitment not found
        [Fact]
        public void UpdateCommitment_NotFound_ThrowsException()
        {
            var dto = new UpdateCommitmentRequest { CommitmentId = 99, Title = "Updated" };
            _commitmentRepoMock.Setup(r => r.GetCommitmentById(99)).Returns((Commitment?)null);

            var ex = Assert.Throws<Exception>(() => _commitmentService.UpdateCommitment(dto));
            Assert.Equal("Commitment not found.", ex.Message);
        }

        // TC-07-01: DeleteCommitment success
        [Fact]
        public void DeleteCommitment_ValidId_DeletesCommitment()
        {
            _commitmentRepoMock.Setup(r => r.GetCommitmentById(1)).Returns(new Commitment { CommitmentId = 1, Title = "Test" });

            _commitmentService.DeleteCommitment(1);

            _commitmentRepoMock.Verify(r => r.DeleteCommitment(1), Times.Once);
        }

        // TC-07-01: DeleteCommitment not found
        [Fact]
        public void DeleteCommitment_NotFound_ThrowsException()
        {
            _commitmentRepoMock.Setup(r => r.GetCommitmentById(99)).Returns((Commitment?)null);

            var ex = Assert.Throws<Exception>(() => _commitmentService.DeleteCommitment(99));
            Assert.Equal("Commitment not found.", ex.Message);
        }
    }
}