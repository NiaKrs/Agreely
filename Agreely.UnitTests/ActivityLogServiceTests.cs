using Agreely.Domain;
using Agreely.Domain.Enums;
using Agreely.Repositories.Interfaces;
using Agreely.Services.Services;
using Moq;

namespace Agreely.UnitTests
{
    public class ActivityLogServiceTests
    {
        private readonly Mock<IActivityLogRepository> _activityLogRepoMock;
        private readonly ActivityLogService _activityLogService;

        public ActivityLogServiceTests()
        {
            _activityLogRepoMock = new Mock<IActivityLogRepository>();
            _activityLogService = new ActivityLogService(_activityLogRepoMock.Object);
        }

        [Fact]
        public void LogEvent_ValidData_CallsRepositoryLogEvent()
        {
            _activityLogService.LogEvent(1, 2, EventTypeValue.CommitmentCreated, "Created commitment");

            _activityLogRepoMock.Verify(r => r.LogEvent(It.Is<ActivityLog>(l =>
                l.GroupId == 1 &&
                l.UserId == 2 &&
                l.EventType == EventTypeValue.CommitmentCreated &&
                l.Description == "Created commitment"
            )), Times.Once);
        }

        [Fact]
        public void GetGroupLog_ValidGroupId_ReturnsMappedList()
        {
            _activityLogRepoMock.Setup(r => r.GetLogsByGroup(1)).Returns(new List<ActivityLog>
            {
                new ActivityLog { LogId = 1, GroupId = 1, UserId = 1, EventType = EventTypeValue.VoteCast, Description = "Voted Agree", UserFullName = "Nia" },
                new ActivityLog { LogId = 2, GroupId = 1, UserId = 2, EventType = EventTypeValue.CommitmentCreated, Description = "Created commitment", UserFullName = "John" }
            });

            var result = _activityLogService.GetGroupLog(1);

            Assert.Equal(2, result.Count);
            Assert.Equal("Voted Agree", result[0].Description);
            Assert.Equal("Nia", result[0].UserFullName);
        }
    }
}