using Agreely.Domain;
using Agreely.Domain.Enums;
using Agreely.Repositories.Interfaces;
using Agreely.Services.Services;
using Moq;

namespace Agreely.UnitTests
{
    public class NotificationServiceTests
    {
        private readonly Mock<INotificationRepository> _notificationRepoMock;
        private readonly Mock<IGroupMembershipRepository> _membershipRepoMock;
        private readonly Mock<IGroupRepository> _groupRepoMock;
        private readonly NotificationService _notificationService;

        public NotificationServiceTests()
        {
            _notificationRepoMock = new Mock<INotificationRepository>();
            _membershipRepoMock = new Mock<IGroupMembershipRepository>();
            _groupRepoMock = new Mock<IGroupRepository>();

            _notificationService = new NotificationService(
                _notificationRepoMock.Object,
                _membershipRepoMock.Object,
                _groupRepoMock.Object
            );
        }


        [Fact]
        public void CreateNotificationsForCommitment_Healthy_DoesNothing()
        {
            _notificationService.CreateNotificationsForCommitment(1, 1, HealthStatusValue.Healthy, "Test");

            _notificationRepoMock.Verify(r => r.CreateNotification(It.IsAny<Notification>()), Times.Never);
            _membershipRepoMock.Verify(r => r.GetMembersByGroupId(It.IsAny<int>()), Times.Never);
        }

        [Fact]
        public void CreateNotificationsForCommitment_NeedsAttention_CreatesNotificationForEachMember()
        {
            _groupRepoMock.Setup(r => r.GetGroupNameById(1)).Returns("Alpha Team");
            _membershipRepoMock.Setup(r => r.GetMembersByGroupId(1)).Returns(new List<int> { 10, 20, 30 });
            _notificationRepoMock.Setup(r => r.NotificationExists(It.IsAny<int>(), 1, HealthStatusValue.NeedsAttention)).Returns(false);

            _notificationService.CreateNotificationsForCommitment(1, 1, HealthStatusValue.NeedsAttention, "Budget plan");

            _notificationRepoMock.Verify(r => r.CreateNotification(It.IsAny<Notification>()), Times.Exactly(3));
        }

        [Fact]
        public void CreateNotificationsForCommitment_DueForReview_CreatesNotificationForEachMember()
        {
            _groupRepoMock.Setup(r => r.GetGroupNameById(1)).Returns("Alpha Team");
            _membershipRepoMock.Setup(r => r.GetMembersByGroupId(1)).Returns(new List<int> { 10, 20 });
            _notificationRepoMock.Setup(r => r.NotificationExists(It.IsAny<int>(), 1, HealthStatusValue.DueForReview)).Returns(false);

            _notificationService.CreateNotificationsForCommitment(1, 1, HealthStatusValue.DueForReview, "Budget plan");

            _notificationRepoMock.Verify(r => r.CreateNotification(It.IsAny<Notification>()), Times.Exactly(2));
        }

        [Fact]
        public void CreateNotificationsForCommitment_DuplicateExists_SkipsThatMember()
        {
            _groupRepoMock.Setup(r => r.GetGroupNameById(1)).Returns("Alpha Team");
            _membershipRepoMock.Setup(r => r.GetMembersByGroupId(1)).Returns(new List<int> { 10, 20 });
            _notificationRepoMock.Setup(r => r.NotificationExists(10, 1, HealthStatusValue.NeedsAttention)).Returns(true);
            _notificationRepoMock.Setup(r => r.NotificationExists(20, 1, HealthStatusValue.NeedsAttention)).Returns(false);

            _notificationService.CreateNotificationsForCommitment(1, 1, HealthStatusValue.NeedsAttention, "Budget plan");

          
            _notificationRepoMock.Verify(r => r.CreateNotification(It.IsAny<Notification>()), Times.Once);
        }

        [Fact]
        public void CreateNotificationsForCommitment_AllDuplicates_CreatesNone()
        {
            _groupRepoMock.Setup(r => r.GetGroupNameById(1)).Returns("Alpha Team");
            _membershipRepoMock.Setup(r => r.GetMembersByGroupId(1)).Returns(new List<int> { 10, 20 });
            _notificationRepoMock.Setup(r => r.NotificationExists(It.IsAny<int>(), 1, HealthStatusValue.NeedsAttention)).Returns(true);

            _notificationService.CreateNotificationsForCommitment(1, 1, HealthStatusValue.NeedsAttention, "Budget plan");

            _notificationRepoMock.Verify(r => r.CreateNotification(It.IsAny<Notification>()), Times.Never);
        }

        [Fact]
        public void CreateNotificationsForCommitment_NeedsAttention_MessageContainsGroupNameAndTitle()
        {
            _groupRepoMock.Setup(r => r.GetGroupNameById(1)).Returns("Alpha Team");
            _membershipRepoMock.Setup(r => r.GetMembersByGroupId(1)).Returns(new List<int> { 10 });
            _notificationRepoMock.Setup(r => r.NotificationExists(10, 1, HealthStatusValue.NeedsAttention)).Returns(false);

            _notificationService.CreateNotificationsForCommitment(1, 1, HealthStatusValue.NeedsAttention, "Budget plan");

            _notificationRepoMock.Verify(r => r.CreateNotification(It.Is<Notification>(n =>
                n.Message.Contains("Alpha Team") &&
                n.Message.Contains("Budget plan") &&
                n.Message.Contains("pending")
            )), Times.Once);
        }

        [Fact]
        public void CreateNotificationsForCommitment_DueForReview_MessageContainsGroupNameAndTitle()
        {
            _groupRepoMock.Setup(r => r.GetGroupNameById(1)).Returns("Alpha Team");
            _membershipRepoMock.Setup(r => r.GetMembersByGroupId(1)).Returns(new List<int> { 10 });
            _notificationRepoMock.Setup(r => r.NotificationExists(10, 1, HealthStatusValue.DueForReview)).Returns(false);

            _notificationService.CreateNotificationsForCommitment(1, 1, HealthStatusValue.DueForReview, "Budget plan");

            _notificationRepoMock.Verify(r => r.CreateNotification(It.Is<Notification>(n =>
                n.Message.Contains("Alpha Team") &&
                n.Message.Contains("Budget plan") &&
                n.Message.Contains("review")
            )), Times.Once);
        }

        [Fact]
        public void CreateNotificationsForCommitment_GroupNameNull_FallsBackToGroupId()
        {
            _groupRepoMock.Setup(r => r.GetGroupNameById(1)).Returns((string?)null);
            _membershipRepoMock.Setup(r => r.GetMembersByGroupId(1)).Returns(new List<int> { 10 });
            _notificationRepoMock.Setup(r => r.NotificationExists(10, 1, HealthStatusValue.NeedsAttention)).Returns(false);

            _notificationService.CreateNotificationsForCommitment(1, 1, HealthStatusValue.NeedsAttention, "Budget plan");

            _notificationRepoMock.Verify(r => r.CreateNotification(It.Is<Notification>(n =>
                n.Message.Contains("Group 1")
            )), Times.Once);
        }

        [Fact]
        public void CreateNotificationsForCommitment_NoMembers_CreatesNoNotifications()
        {
            _groupRepoMock.Setup(r => r.GetGroupNameById(1)).Returns("Alpha Team");
            _membershipRepoMock.Setup(r => r.GetMembersByGroupId(1)).Returns(new List<int>());

            _notificationService.CreateNotificationsForCommitment(1, 1, HealthStatusValue.NeedsAttention, "Budget plan");

            _notificationRepoMock.Verify(r => r.CreateNotification(It.IsAny<Notification>()), Times.Never);
        }

        [Fact]
        public void CreateNotificationsForCommitment_NotificationHasCorrectFields()
        {
            _groupRepoMock.Setup(r => r.GetGroupNameById(2)).Returns("Beta Team");
            _membershipRepoMock.Setup(r => r.GetMembersByGroupId(2)).Returns(new List<int> { 5 });
            _notificationRepoMock.Setup(r => r.NotificationExists(5, 3, HealthStatusValue.DueForReview)).Returns(false);

            _notificationService.CreateNotificationsForCommitment(3, 2, HealthStatusValue.DueForReview, "Agreement X");

            _notificationRepoMock.Verify(r => r.CreateNotification(It.Is<Notification>(n =>
                n.UserId == 5 &&
                n.GroupId == 2 &&
                n.CommitmentId == 3 &&
                n.HealthStatus == HealthStatusValue.DueForReview &&
                n.IsRead == false
            )), Times.Once);
        }

        

        [Fact]
        public void GetNotificationsForUser_ReturnsAllMapped()
        {
            _notificationRepoMock.Setup(r => r.GetNotificationsByUserId(1)).Returns(new List<Notification>
            {
                new Notification { NotificationId = 1, UserId = 1, GroupId = 1, CommitmentId = 1, HealthStatus = HealthStatusValue.NeedsAttention, Message = "msg", IsRead = false, CreatedAt = DateTime.Now },
                new Notification { NotificationId = 2, UserId = 1, GroupId = 1, CommitmentId = 2, HealthStatus = HealthStatusValue.DueForReview,   Message = "msg", IsRead = true,  CreatedAt = DateTime.Now }
            });

            var result = _notificationService.GetNotificationsForUser(1);

            Assert.Equal(2, result.Count);
            Assert.Equal(1, result[0].NotificationId);
            Assert.Equal(2, result[1].NotificationId);
        }

        [Fact]
        public void GetNotificationsForUser_NoNotifications_ReturnsEmptyList()
        {
            _notificationRepoMock.Setup(r => r.GetNotificationsByUserId(99)).Returns(new List<Notification>());

            var result = _notificationService.GetNotificationsForUser(99);

            Assert.Empty(result);
        }

        

        [Fact]
        public void GetUnreadCount_ReturnsCountFromRepo()
        {
            _notificationRepoMock.Setup(r => r.GetUnreadCount(1)).Returns(3);

            var result = _notificationService.GetUnreadCount(1);

            Assert.Equal(3, result);
        }

        [Fact]
        public void GetUnreadCount_NoUnread_ReturnsZero()
        {
            _notificationRepoMock.Setup(r => r.GetUnreadCount(1)).Returns(0);

            var result = _notificationService.GetUnreadCount(1);

            Assert.Equal(0, result);
        }

       

        [Fact]
        public void MarkAsRead_CallsRepository()
        {
            _notificationService.MarkAsRead(5);

            _notificationRepoMock.Verify(r => r.MarkAsRead(5), Times.Once);
        }


        [Fact]
        public void GetByIdForUser_Exists_ReturnsMappedResponse()
        {
            _notificationRepoMock.Setup(r => r.GetNotificationByIdForUser(1, 10)).Returns(new Notification
            {
                NotificationId = 1,
                UserId = 10,
                GroupId = 2,
                CommitmentId = 3,
                HealthStatus = HealthStatusValue.NeedsAttention,
                Message = "test",
                IsRead = false,
                CreatedAt = DateTime.Now
            });

            var result = _notificationService.GetByIdForUser(1, 10);

            Assert.NotNull(result);
            Assert.Equal(1, result.NotificationId);
            Assert.Equal(2, result.GroupId);
            Assert.Equal(3, result.CommitmentId);
            Assert.False(result.IsRead);
        }

        [Fact]
        public void GetByIdForUser_NotFound_ReturnsNull()
        {
            _notificationRepoMock.Setup(r => r.GetNotificationByIdForUser(99, 10)).Returns((Notification?)null);

            var result = _notificationService.GetByIdForUser(99, 10);

            Assert.Null(result);
        }

        [Fact]
        public void GetByIdForUser_WrongUser_ReturnsNull()
        {
            _notificationRepoMock.Setup(r => r.GetNotificationByIdForUser(1, 99)).Returns((Notification?)null);

            var result = _notificationService.GetByIdForUser(1, 99);

            Assert.Null(result);
        }
    }
}