using Agreely.Repositories.Interfaces;
using Agreely.Services.DTO.Requests;
using Agreely.Services.DTO.Responses;
using Agreely.Domain;
using Agreely.Services.Interfaces;

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
                Title = request.Title,
                Description = request.Description
            };

            return _commitmentRepo.CreateCommitment(commitment);
        }

        public List<ViewCommitmentResponse> GetCommitmentsByGroupId(int groupId)
        {
            return _commitmentRepo.GetCommitmentsByGroupId(groupId)
                .Select(c => new ViewCommitmentResponse
                {
                    CommitmentId = c.CommitmentId,
                    GroupId = c.GroupId,
                    Title = c.Title,
                    Description = c.Description
                }).ToList();
        }


        public ViewCommitmentResponse? GetCommitmentById(int commitmentId)
        {
            var commitment = _commitmentRepo.GetCommitmentById(commitmentId);
            if (commitment == null) return null;

            return new ViewCommitmentResponse
            {
                CommitmentId = commitment.CommitmentId,
                GroupId = commitment.GroupId,
                Title = commitment.Title,
                Description = commitment.Description
            };
        }

        public void UpdateCommitment(UpdateCommitmentRequest request)
        {
            var commitment = _commitmentRepo.GetCommitmentById(request.CommitmentId);
            if (commitment == null)
                throw new Exception("Commitment not found.");

            commitment.Title = request.Title;
            commitment.Description = request.Description;

            _commitmentRepo.UpdateCommitment(commitment);
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