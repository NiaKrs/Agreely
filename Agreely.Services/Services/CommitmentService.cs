using Agreely.Repositories.Interfaces;
using Agreely.Services.DTO.Requests;
using Agreely.Services.DTO.Responses;
using Agreely.Domain;
using Agreely.Services.Interfaces;
using Agreely.Domain.Enums;
using System.Transactions;

namespace Agreely.Services.Services
{
    public class CommitmentService : ICommitmentService
    {
        private readonly ICommitmentRepository _commitmentRepo;
        private readonly IGroupMembershipRepository _membershipRepo;
        private readonly IActivityLogService _activityLogService;
        private readonly HealthStatusEvaluator _healthStatusEvaluator;

        public CommitmentService(ICommitmentRepository commitmentRepo, IGroupMembershipRepository membershipRepo, IActivityLogService activityLogService, HealthStatusEvaluator healthStatusEvaluator)
        {
            _commitmentRepo = commitmentRepo;
            _membershipRepo = membershipRepo;
            _activityLogService = activityLogService;
            _healthStatusEvaluator = healthStatusEvaluator;
        }

        public int CreateCommitment(CreateCommitmentRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Title))
                throw new Exception("Commitment title is required.");

            if (!_membershipRepo.IsMember(request.GroupId, request.CreatedByUserId))
                throw new Exception("You must be a member of the group to create a commitment.");

            var commitment = new Commitment
            {
                GroupId = request.GroupId,
                CreatedByUserId = request.CreatedByUserId,
                Status = CommitmentStatus.Pending,
            };

            var version = new CommitmentVersion
            {
                CreatedByUserId = request.CreatedByUserId,
                Title = request.Title,
                Description = request.Description,
                IsActive = true,
                CreatedAt = DateTime.Now
            };

            int commitmentId;
            try
            {
                using (var scope = new TransactionScope())
                {
                    commitmentId = _commitmentRepo.InsertCommitment(commitment);
                    version.CommitmentId = commitmentId;
                    _commitmentRepo.CreateCommitmentVersion(version);
                    scope.Complete();
                }
            }
            catch (Exception)
            {
                throw new Exception("Failed to create commitment. All changes have been rolled back.");
            }

            _activityLogService.LogEvent(request.GroupId, request.CreatedByUserId, EventTypeValue.CommitmentCreated, $"Created commitment \"{request.Title}\"");
            return commitmentId;
        }

        public int CreateCommitmentVersion(CreateCommitmentVersionRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Title))
                throw new Exception("Commitment title is required.");

            var commitment = _commitmentRepo.GetCommitmentById(request.CommitmentId);
            if (commitment == null)
                throw new Exception("Commitment not found.");

            var version = new CommitmentVersion
            {
                CommitmentId = request.CommitmentId,
                Title = request.Title,
                Description = request.Description,
                CreatedByUserId = request.CreatedByUserId,
                IsActive = true,
                CreatedAt = DateTime.Now
            };

            int newVersionId;
            try
            {
                using (var scope = new TransactionScope())
                {
                    _commitmentRepo.DeactivatePreviousVersions(request.CommitmentId);
                    _commitmentRepo.UpdateCommitmentStatus(request.CommitmentId, CommitmentStatus.Pending);
                    newVersionId = _commitmentRepo.CreateCommitmentVersion(version);
                    scope.Complete();
                }
            }
            catch (Exception)
            {
                throw new Exception("Failed to update commitment. All changes have been rolled back.");
            }

            _activityLogService.LogEvent(commitment.GroupId, request.CreatedByUserId, EventTypeValue.CommitmentRevised, $"Revised commitment to \"{request.Title}\"");
            return newVersionId;
        }

        public List<ViewCommitmentResponse> GetCommitmentsByGroupId(int groupId)
        {
            return _commitmentRepo.GetCommitmentsByGroupId(groupId)
                .Select(c =>
                {
                    var version = _commitmentRepo.GetCurrentVersion(c.CommitmentId);
                    return new ViewCommitmentResponse
                    {
                        CommitmentId = c.CommitmentId,
                        Title = version?.Title ?? string.Empty,
                        Description = version?.Description,
                        Status = c.Status,
                        CommitmentVersionId = version?.Id ?? 0,
                        HealthStatus = version != null
                            ? _healthStatusEvaluator.Evaluate(c.Status, version.CreatedAt)
                            : HealthStatusValue.Healthy,
                        CreatedAt = version?.CreatedAt ?? DateTime.MinValue
                    };
                }).ToList();
        }

        public ViewCommitmentResponse? GetCommitmentById(int commitmentId)
        {
            var commitment = _commitmentRepo.GetCommitmentById(commitmentId);
            if (commitment == null) return null;

            var version = _commitmentRepo.GetCurrentVersion(commitmentId);
            return new ViewCommitmentResponse
            {
                CommitmentId = commitment.CommitmentId,
                Title = version?.Title ?? string.Empty,
                Description = version?.Description,
                Status = commitment.Status,
                CommitmentVersionId = version?.Id ?? 0,
                HealthStatus = version != null
                            ? _healthStatusEvaluator.Evaluate(commitment.Status, version.CreatedAt)
                            : HealthStatusValue.Healthy
            };
        }

        public void DeleteCommitment(int commitmentId)
        {
            var commitment = _commitmentRepo.GetCommitmentById(commitmentId);
            if (commitment == null)
                throw new Exception("Commitment not found.");

            try
            {
                using (var scope = new TransactionScope())
                {
                    _commitmentRepo.DeleteVotesByCommitmentId(commitmentId);
                    _commitmentRepo.DeleteNotificationsByCommitmentId(commitmentId);
                    _commitmentRepo.DeleteVersionsByCommitmentId(commitmentId);
                    _commitmentRepo.DeleteCommitment(commitmentId);
                    scope.Complete();
                }
            }
            catch (Exception)
            {
                throw new Exception("Failed to delete commitment. All changes have been rolled back.");
            }
        }

        public void RequestReview(int commitmentId, int requestedByUserId)
        {
            var commitment = _commitmentRepo.GetCommitmentById(commitmentId);
            if (commitment == null)
                throw new Exception("Commitment not found.");

            var version = _commitmentRepo.GetCurrentVersion(commitmentId);
            if (version == null)
                throw new Exception("No active version found for this commitment.");

            try
            {
                using (var scope = new TransactionScope())
                {
                    _commitmentRepo.DeleteVotesByCommitmentId(commitmentId);
                    _commitmentRepo.UpdateCommitmentStatus(commitmentId, CommitmentStatus.Pending);
                    scope.Complete();
                }
            }
            catch (Exception)
            {
                throw new Exception("Failed to request review. All changes have been rolled back.");
            }

            _activityLogService.LogEvent(commitment.GroupId, requestedByUserId,
                EventTypeValue.ReviewRequested,
                $"Requested review of commitment \"{version.Title}\"");
        }
    }
}