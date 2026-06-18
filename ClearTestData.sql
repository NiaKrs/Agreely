/* =========================================================
   Agreely - Clear Test Data
   Run only on development/testing database
   ========================================================= */

-- children first
DELETE FROM [Notification];
DELETE FROM [AlignmentVote];
DELETE FROM [CommitmentVersion];
DELETE FROM [Commitment];
DELETE FROM [ActivityLog];
DELETE FROM [GroupMembership];

-- parents last
DELETE FROM [Group];
DELETE FROM [User];

-- reset identity values
DBCC CHECKIDENT ('[Notification]', RESEED, 0);
DBCC CHECKIDENT ('[AlignmentVote]', RESEED, 0);
DBCC CHECKIDENT ('[CommitmentVersion]', RESEED, 0);
DBCC CHECKIDENT ('[Commitment]', RESEED, 0);
DBCC CHECKIDENT ('[ActivityLog]', RESEED, 0);
DBCC CHECKIDENT ('[GroupMembership]', RESEED, 0);
DBCC CHECKIDENT ('[Group]', RESEED, 0);
DBCC CHECKIDENT ('[User]', RESEED, 0);