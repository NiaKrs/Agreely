using Agreely.Domain;
using Agreely.Domain.Enums;
using Agreely.Repositories.Interfaces;
using Agreely.Services.DTO.Requests;
using Agreely.Services.Interfaces;
using Agreely.Services.Services;
using Moq;

namespace Agreely.UnitTests
{
    public class VoteServiceTests
    {
        private readonly Mock<IVoteRepository> _voteRepoMock;
        private readonly Mock<ICommitmentRepository> _commitmentRepoMock;
        private readonly Mock<IGroupRepository> _groupRepoMock;
        private readonly Mock<IActivityLogService> _activityLogServiceMock;
        private readonly VoteService _voteService;

        public VoteServiceTests()
        {
            _voteRepoMock = new Mock<IVoteRepository>();
            _commitmentRepoMock = new Mock<ICommitmentRepository>();
            _groupRepoMock = new Mock<IGroupRepository>();
            _activityLogServiceMock = new Mock<IActivityLogService>();
            _voteService = new VoteService(
                _voteRepoMock.Object,
                _commitmentRepoMock.Object,
                _groupRepoMock.Object,
                _activityLogServiceMock.Object
            );
        }

        [Fact]
        public void CastOrUpdateVote_NewVote_CallsCastVoteAndLogsVoteCast()
        {
            var request = new CastVoteRequest { CommitmentVersionId = 1, CommitmentId = 1, GroupId = 1, UserId = 1, Vote = VoteValue.Agree };
            _voteRepoMock.Setup(r => r.GetVote(1, 1)).Returns((AlignmentVote?)null);
            _voteRepoMock.Setup(r => r.GetVotesByVersion(1)).Returns(new List<AlignmentVote>());
            _commitmentRepoMock.Setup(r => r.GetCurrentVersion(1)).Returns(new CommitmentVersion { Title = "Test" });
            _commitmentRepoMock.Setup(r => r.GetCommitmentById(1)).Returns(new Commitment { CommitmentId = 1, Status = CommitmentStatus.Pending });
            _groupRepoMock.Setup(r => r.GetMemberCount(1)).Returns(3);

            _voteService.CastOrUpdateVote(request);

            _voteRepoMock.Verify(r => r.CastVote(It.IsAny<AlignmentVote>()), Times.Once);
            _activityLogServiceMock.Verify(a => a.LogEvent(1, 1, EventTypeValue.VoteCast, It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public void CastOrUpdateVote_ExistingVote_CallsUpdateVoteAndLogsVoteChanged()
        {
            var request = new CastVoteRequest { CommitmentVersionId = 1, CommitmentId = 1, GroupId = 1, UserId = 1, Vote = VoteValue.Disagree };
            _voteRepoMock.Setup(r => r.GetVote(1, 1)).Returns(new AlignmentVote { CommitmentVersionId = 1, UserId = 1, Vote = VoteValue.Agree });
            _voteRepoMock.Setup(r => r.GetVotesByVersion(1)).Returns(new List<AlignmentVote>());
            _commitmentRepoMock.Setup(r => r.GetCurrentVersion(1)).Returns(new CommitmentVersion { Title = "Test" });
            _commitmentRepoMock.Setup(r => r.GetCommitmentById(1)).Returns(new Commitment { CommitmentId = 1, Status = CommitmentStatus.Pending });
            _groupRepoMock.Setup(r => r.GetMemberCount(1)).Returns(3);

            _voteService.CastOrUpdateVote(request);

            _voteRepoMock.Verify(r => r.UpdateVote(It.IsAny<AlignmentVote>()), Times.Once);
            _activityLogServiceMock.Verify(a => a.LogEvent(1, 1, EventTypeValue.VoteChanged, It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public void CastOrUpdateVote_AllMembersAgree_SetsStatusActiveAndLogsStatusChanged()
        {
            var request = new CastVoteRequest { CommitmentVersionId = 1, CommitmentId = 1, GroupId = 1, UserId = 1, Vote = VoteValue.Agree };
            _voteRepoMock.Setup(r => r.GetVote(1, 1)).Returns((AlignmentVote?)null);
            _voteRepoMock.Setup(r => r.GetVotesByVersion(1)).Returns(new List<AlignmentVote>
            {
                new AlignmentVote { Vote = VoteValue.Agree },
                new AlignmentVote { Vote = VoteValue.Agree }
            });
            _commitmentRepoMock.Setup(r => r.GetCurrentVersion(1)).Returns(new CommitmentVersion { Title = "Test" });
            _commitmentRepoMock.Setup(r => r.GetCommitmentById(1)).Returns(new Commitment { CommitmentId = 1, Status = CommitmentStatus.Pending });
            _groupRepoMock.Setup(r => r.GetMemberCount(1)).Returns(2);

            _voteService.CastOrUpdateVote(request);

            _commitmentRepoMock.Verify(r => r.UpdateCommitmentStatus(1, CommitmentStatus.Active), Times.Once);
            _activityLogServiceMock.Verify(a => a.LogEvent(1, 1, EventTypeValue.StatusChanged, It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public void CastOrUpdateVote_NotAllMembersAgree_StatusRemainsAndNoStatusLog()
        {
            var request = new CastVoteRequest { CommitmentVersionId = 1, CommitmentId = 1, GroupId = 1, UserId = 1, Vote = VoteValue.Agree };
            _voteRepoMock.Setup(r => r.GetVote(1, 1)).Returns((AlignmentVote?)null);
            _voteRepoMock.Setup(r => r.GetVotesByVersion(1)).Returns(new List<AlignmentVote>
            {
                new AlignmentVote { Vote = VoteValue.Agree }
            });
            _commitmentRepoMock.Setup(r => r.GetCurrentVersion(1)).Returns(new CommitmentVersion { Title = "Test" });
            _commitmentRepoMock.Setup(r => r.GetCommitmentById(1)).Returns(new Commitment { CommitmentId = 1, Status = CommitmentStatus.Pending });
            _groupRepoMock.Setup(r => r.GetMemberCount(1)).Returns(3);

            _voteService.CastOrUpdateVote(request);

            _commitmentRepoMock.Verify(r => r.UpdateCommitmentStatus(1, CommitmentStatus.Pending), Times.Once);
            _activityLogServiceMock.Verify(a => a.LogEvent(1, 1, EventTypeValue.StatusChanged, It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public void GetUserVote_VoteExists_ReturnsVoteValue()
        {
            _voteRepoMock.Setup(r => r.GetVote(1, 1)).Returns(new AlignmentVote { Vote = VoteValue.Agree });

            var result = _voteService.GetUserVote(1, 1);

            Assert.Equal(VoteValue.Agree, result);
        }

        [Fact]
        public void GetUserVote_NoVote_ReturnsNull()
        {
            _voteRepoMock.Setup(r => r.GetVote(1, 1)).Returns((AlignmentVote?)null);

            var result = _voteService.GetUserVote(1, 1);

            Assert.Null(result);
        }

        [Fact]
        public void GetVoteCounts_ReturnsCorrectCounts()
        {
            _voteRepoMock.Setup(r => r.GetVotesByVersion(1)).Returns(new List<AlignmentVote>
            {
                new AlignmentVote { Vote = VoteValue.Agree },
                new AlignmentVote { Vote = VoteValue.Agree },
                new AlignmentVote { Vote = VoteValue.Neutral },
                new AlignmentVote { Vote = VoteValue.Disagree }
            });

            var result = _voteService.GetVoteCounts(1);

            Assert.Equal(2, result.Agree);
            Assert.Equal(1, result.Neutral);
            Assert.Equal(1, result.Disagree);
            Assert.Equal(4, result.Total);
        }
    }
}