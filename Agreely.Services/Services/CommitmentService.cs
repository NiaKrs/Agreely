using Agreely.Repositories.Interfaces;
using Agreely.Services.DTO.Requests;
using Agreely.Services.DTO.Responses;
using Agreely.Domain;
using Agreely.Services.Interfaces;
using Agreely.Domain.Enums;

namespace Agreely.Services.Services
{
    public class CommitmentService : ICommitmentService
    {
        private readonly ICommitmentRepository _commitmentRepo;
        private readonly IGroupMembershipRepository _membershipRepo;

        public CommitmentService(ICommitmentRepository commitmentRepo, IGroupMembershipRepository membershipRepo)
        {
            _commitmentRepo = commitmentRepo;
            _membershipRepo = membershipRepo;
        }

        public int CreateCommitment(CreateCommitmentRequest request)
        {
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
                CreatedAt = DateTime.UtcNow
            };

            return _commitmentRepo.CreateCommitment(commitment, version);
        }

        public int CreateCommitmentVersion(CreateCommitmentVersionRequest request)
        {
            var commitment = _commitmentRepo.GetCommitmentById(request.CommitmentId);
            if (commitment == null)
                throw new Exception("Commitment not found.");

            _commitmentRepo.DeactivatePreviousVersions(request.CommitmentId);
            var version = new CommitmentVersion
            {
                CommitmentId = request.CommitmentId,
                Title = request.Title,
                Description = request.Description,
                CreatedByUserId = request.CreatedByUserId,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            _commitmentRepo.UpdateCommitmentStatus(request.CommitmentId, CommitmentStatus.Pending);
            return _commitmentRepo.CreateCommitmentVersion(version);
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
                        CommitmentVersionId = version?.Id ?? 0
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
                CommitmentVersionId = version?.Id ?? 0
            };
        }


        public void DeleteCommitment(int commitmentId)
        {
            var commitment = _commitmentRepo.GetCommitmentById(commitmentId);
            if (commitment == null)
                throw new Exception("Commitment not found.");

            _commitmentRepo.DeleteCommitment(commitmentId);
        }
    }
}