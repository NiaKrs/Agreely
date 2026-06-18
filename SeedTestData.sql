
/* =========================================================
   Agreely - Seed Test Data
   Run only on development/testing database
   Run ClearTestData.sql before this script
   ========================================================= */

BEGIN TRANSACTION;

------------------------------------------------------------
-- 1. USERS
------------------------------------------------------------

-- Replace this with a real BCrypt hash for password: Test1234!
DECLARE @Password NVARCHAR(255) = '$2a$11$Jcex6CYlxKCp5mxdbxzPkuj1ULgbErpB11ZLv7i5Lv5aOCcAiIbvS';

INSERT INTO [User] (FullName, Email, Password)
VALUES ('Test User A', 'usera@test.com', @Password);
DECLARE @UserAId INT = SCOPE_IDENTITY();

INSERT INTO [User] (FullName, Email, Password)
VALUES ('Test User B', 'userb@test.com', @Password);
DECLARE @UserBId INT = SCOPE_IDENTITY();

INSERT INTO [User] (FullName, Email, Password)
VALUES ('Test User C', 'userc@test.com', @Password);
DECLARE @UserCId INT = SCOPE_IDENTITY();

------------------------------------------------------------
-- 2. GROUPS
------------------------------------------------------------

INSERT INTO [Group] (Name, Description, CreatedByUserId, CreatedAt)
VALUES ('Alpha Group', 'Main group used for general feature testing.', @UserAId, GETDATE());
DECLARE @AlphaGroupId INT = SCOPE_IDENTITY();

INSERT INTO [Group] (Name, Description, CreatedByUserId, CreatedAt)
VALUES ('Empty Activity Group', 'Used for activity log empty-state testing.', @UserAId, GETDATE());
DECLARE @EmptyActivityGroupId INT = SCOPE_IDENTITY();

INSERT INTO [Group] (Name, Description, CreatedByUserId, CreatedAt)
VALUES ('Empty Commitments Group', 'Used for empty commitment and health panel testing.', @UserAId, GETDATE());
DECLARE @EmptyCommitmentsGroupId INT = SCOPE_IDENTITY();

INSERT INTO [Group] (Name, Description, CreatedByUserId, CreatedAt)
VALUES ('Two Member Voting Group', 'Used for voting status transition tests.', @UserAId, GETDATE());
DECLARE @TwoMemberGroupId INT = SCOPE_IDENTITY();

INSERT INTO [Group] (Name, Description, CreatedByUserId, CreatedAt)
VALUES ('Health Testing Group', 'Used for commitment health, review, summary, and notification tests.', @UserAId, GETDATE());
DECLARE @HealthGroupId INT = SCOPE_IDENTITY();

------------------------------------------------------------
-- 3. GROUP MEMBERSHIPS
------------------------------------------------------------

INSERT INTO [GroupMembership] (GroupId, UserId, JoinedAt)
VALUES
(@AlphaGroupId, @UserAId, GETDATE()),
(@AlphaGroupId, @UserBId, GETDATE()),

(@EmptyActivityGroupId, @UserAId, GETDATE()),
(@EmptyCommitmentsGroupId, @UserAId, GETDATE()),

(@TwoMemberGroupId, @UserAId, GETDATE()),
(@TwoMemberGroupId, @UserBId, GETDATE()),

(@HealthGroupId, @UserAId, GETDATE()),
(@HealthGroupId, @UserBId, GETDATE());

------------------------------------------------------------
-- 4. COMMITMENTS
------------------------------------------------------------

-- CommitmentStatus:
-- Pending = 0
-- Active = 1

-- Alpha Group commitments
INSERT INTO [Commitment] (GroupId, CreatedByUserId, CreatedAt, Status)
VALUES (@AlphaGroupId, @UserAId, GETDATE(), 0);
DECLARE @SampleCommitmentId INT = SCOPE_IDENTITY();

INSERT INTO [Commitment] (GroupId, CreatedByUserId, CreatedAt, Status)
VALUES (@AlphaGroupId, @UserAId, GETDATE(), 0);
DECLARE @EditVersionCommitmentId INT = SCOPE_IDENTITY();

INSERT INTO [Commitment] (GroupId, CreatedByUserId, CreatedAt, Status)
VALUES (@AlphaGroupId, @UserAId, GETDATE(), 0);
DECLARE @CastVoteCommitmentId INT = SCOPE_IDENTITY();

INSERT INTO [Commitment] (GroupId, CreatedByUserId, CreatedAt, Status)
VALUES (@AlphaGroupId, @UserAId, GETDATE(), 0);
DECLARE @ChangeVoteCommitmentId INT = SCOPE_IDENTITY();

INSERT INTO [Commitment] (GroupId, CreatedByUserId, CreatedAt, Status)
VALUES (@AlphaGroupId, @UserAId, GETDATE(), 0);
DECLARE @DeleteCommitmentId INT = SCOPE_IDENTITY();

INSERT INTO [Commitment] (GroupId, CreatedByUserId, CreatedAt, Status)
VALUES (@AlphaGroupId, @UserAId, GETDATE(), 0);
DECLARE @VoteLogCommitmentId INT = SCOPE_IDENTITY();

INSERT INTO [Commitment] (GroupId, CreatedByUserId, CreatedAt, Status)
VALUES (@AlphaGroupId, @UserAId, GETDATE(), 0);
DECLARE @RevisionLogCommitmentId INT = SCOPE_IDENTITY();

-- Two Member Voting Group commitments
INSERT INTO [Commitment] (GroupId, CreatedByUserId, CreatedAt, Status)
VALUES (@TwoMemberGroupId, @UserAId, GETDATE(), 0);
DECLARE @AgreementActivationCommitmentId INT = SCOPE_IDENTITY();

INSERT INTO [Commitment] (GroupId, CreatedByUserId, CreatedAt, Status)
VALUES (@TwoMemberGroupId, @UserAId, GETDATE(), 0);
DECLARE @PendingAlignmentCommitmentId INT = SCOPE_IDENTITY();

INSERT INTO [Commitment] (GroupId, CreatedByUserId, CreatedAt, Status)
VALUES (@TwoMemberGroupId, @UserAId, GETDATE(), 0);
DECLARE @StatusLogCommitmentId INT = SCOPE_IDENTITY();

-- Health Testing Group commitments
INSERT INTO [Commitment] (GroupId, CreatedByUserId, CreatedAt, Status)
VALUES (@HealthGroupId, @UserAId, GETDATE(), 0);
DECLARE @FreshCommitmentId INT = SCOPE_IDENTITY();

INSERT INTO [Commitment] (GroupId, CreatedByUserId, CreatedAt, Status)
VALUES (@HealthGroupId, @UserAId, DATEADD(DAY, -8, GETDATE()), 0);
DECLARE @StalePendingCommitmentId INT = SCOPE_IDENTITY();

INSERT INTO [Commitment] (GroupId, CreatedByUserId, CreatedAt, Status)
VALUES (@HealthGroupId, @UserAId, DATEADD(DAY, -31, GETDATE()), 1);
DECLARE @OldActiveCommitmentId INT = SCOPE_IDENTITY();

------------------------------------------------------------
-- 5. COMMITMENT VERSIONS
------------------------------------------------------------

INSERT INTO [CommitmentVersion] (CommitmentId, CreatedByUserId, Title, Description, CreatedAt, IsActive)
VALUES (@SampleCommitmentId, @UserAId, 'Sample Commitment', 'Seeded commitment for general viewing.', GETDATE(), 1);
DECLARE @SampleVersionId INT = SCOPE_IDENTITY();

INSERT INTO [CommitmentVersion] (CommitmentId, CreatedByUserId, Title, Description, CreatedAt, IsActive)
VALUES (@EditVersionCommitmentId, @UserAId, 'Edit Version Test', 'Used to test commitment versioning and vote reset.', GETDATE(), 1);
DECLARE @EditVersionId INT = SCOPE_IDENTITY();

INSERT INTO [CommitmentVersion] (CommitmentId, CreatedByUserId, Title, Description, CreatedAt, IsActive)
VALUES (@CastVoteCommitmentId, @UserAId, 'Cast Vote Test', 'Used to test casting a first vote.', GETDATE(), 1);
DECLARE @CastVoteVersionId INT = SCOPE_IDENTITY();

INSERT INTO [CommitmentVersion] (CommitmentId, CreatedByUserId, Title, Description, CreatedAt, IsActive)
VALUES (@ChangeVoteCommitmentId, @UserAId, 'Change Vote Test', 'Used to test changing an existing vote.', GETDATE(), 1);
DECLARE @ChangeVoteVersionId INT = SCOPE_IDENTITY();

INSERT INTO [CommitmentVersion] (CommitmentId, CreatedByUserId, Title, Description, CreatedAt, IsActive)
VALUES (@DeleteCommitmentId, @UserAId, 'Commitment To Delete', 'Used to test cascade deletion.', GETDATE(), 1);
DECLARE @DeleteVersionId INT = SCOPE_IDENTITY();

INSERT INTO [CommitmentVersion] (CommitmentId, CreatedByUserId, Title, Description, CreatedAt, IsActive)
VALUES (@VoteLogCommitmentId, @UserAId, 'Vote Log Test', 'Used to test activity logging for votes.', GETDATE(), 1);
DECLARE @VoteLogVersionId INT = SCOPE_IDENTITY();

INSERT INTO [CommitmentVersion] (CommitmentId, CreatedByUserId, Title, Description, CreatedAt, IsActive)
VALUES (@RevisionLogCommitmentId, @UserAId, 'Revision Log Test', 'Used to test activity logging for revisions.', GETDATE(), 1);
DECLARE @RevisionLogVersionId INT = SCOPE_IDENTITY();

INSERT INTO [CommitmentVersion] (CommitmentId, CreatedByUserId, Title, Description, CreatedAt, IsActive)
VALUES (@AgreementActivationCommitmentId, @UserAId, 'Agreement Activation Test', 'Used to test Active status after all members agree.', GETDATE(), 1);
DECLARE @AgreementActivationVersionId INT = SCOPE_IDENTITY();

INSERT INTO [CommitmentVersion] (CommitmentId, CreatedByUserId, Title, Description, CreatedAt, IsActive)
VALUES (@PendingAlignmentCommitmentId, @UserAId, 'Pending Alignment Test', 'Used to test Pending status when one member disagrees.', GETDATE(), 1);
DECLARE @PendingAlignmentVersionId INT = SCOPE_IDENTITY();

INSERT INTO [CommitmentVersion] (CommitmentId, CreatedByUserId, Title, Description, CreatedAt, IsActive)
VALUES (@StatusLogCommitmentId, @UserAId, 'Status Log Test', 'Used to test activity logging for status changes.', GETDATE(), 1);
DECLARE @StatusLogVersionId INT = SCOPE_IDENTITY();

INSERT INTO [CommitmentVersion] (CommitmentId, CreatedByUserId, Title, Description, CreatedAt, IsActive)
VALUES (@FreshCommitmentId, @UserAId, 'Fresh Commitment', 'Should display as Healthy.', GETDATE(), 1);
DECLARE @FreshVersionId INT = SCOPE_IDENTITY();

INSERT INTO [CommitmentVersion] (CommitmentId, CreatedByUserId, Title, Description, CreatedAt, IsActive)
VALUES (@StalePendingCommitmentId, @UserAId, 'Stale Pending Commitment', 'Should display as Needs Attention.', DATEADD(DAY, -8, GETDATE()), 1);
DECLARE @StalePendingVersionId INT = SCOPE_IDENTITY();

INSERT INTO [CommitmentVersion] (CommitmentId, CreatedByUserId, Title, Description, CreatedAt, IsActive)
VALUES (@OldActiveCommitmentId, @UserAId, 'Old Active Commitment', 'Should display as Due For Review.', DATEADD(DAY, -31, GETDATE()), 1);
DECLARE @OldActiveVersionId INT = SCOPE_IDENTITY();

------------------------------------------------------------
-- 6. VOTES
------------------------------------------------------------

-- VoteValue:
-- Disagree = 0
-- Neutral = 1
-- Agree = 2

INSERT INTO [AlignmentVote] (CommitmentVersionId, UserId, Vote, CreatedAt)
VALUES
(@EditVersionId, @UserAId, 2, GETDATE()),
(@ChangeVoteVersionId, @UserAId, 2, GETDATE()),
(@DeleteVersionId, @UserAId, 2, GETDATE()),

(@OldActiveVersionId, @UserAId, 2, DATEADD(DAY, -31, GETDATE())),
(@OldActiveVersionId, @UserBId, 2, DATEADD(DAY, -31, GETDATE()));

------------------------------------------------------------
-- 7. ACTIVITY LOG
------------------------------------------------------------

-- EventTypeValue:
-- CommitmentCreated = 0
-- CommitmentRevised = 1
-- VoteCast = 2
-- VoteChanged = 3
-- StatusChanged = 4
-- ReviewRequested = 5

INSERT INTO [ActivityLog] (GroupId, UserId, EventType, OccuredAt, Description)
VALUES
(@AlphaGroupId, @UserAId, 0, DATEADD(MINUTE, -30, GETDATE()), 'Seeded activity: commitment was created.'),
(@AlphaGroupId, @UserAId, 2, DATEADD(MINUTE, -20, GETDATE()), 'Seeded activity: vote was cast.'),
(@AlphaGroupId, @UserBId, 4, DATEADD(MINUTE, -10, GETDATE()), 'Seeded activity: commitment status changed.');

------------------------------------------------------------
-- 8. NOTIFICATIONS
------------------------------------------------------------

-- HealthStatusValue:
-- Healthy = 0
-- NeedsAttention = 1
-- DueForReview = 2

INSERT INTO [Notification] (GroupId, UserId, CommitmentId, Message, HealthStatus, CreatedAt, IsRead)
VALUES
(@AlphaGroupId, @UserAId, @DeleteCommitmentId, '[Group: Alpha Group] Commitment "Commitment To Delete" needs attention.', 1, GETDATE(), 0);

COMMIT TRANSACTION;
