using Agreely.Repositories.Interfaces;
using Agreely.Repositories.Models;
using Agreely.Services.DTO;
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

        public int CreateCommitment(CreateCommitmentDto dto)
        {
            if (!_membershipRepo.IsMember(dto.GroupId, dto.CreatedByUserId))
                throw new Exception("You must be a member of the group to create a commitment.");

            var commitment = new Commitment
            {
                GroupId = dto.GroupId,
                CreatedByUserId = dto.CreatedByUserId,
                Title = dto.Title,
                Description = dto.Description
            };

            return _commitmentRepo.CreateCommitment(commitment);
        }

        public List<Commitment> GetCommitmentsByGroupId(int groupId)
        {
            return _commitmentRepo.GetCommitmentsByGroupId(groupId);
        }


        public Commitment? GetCommitmentById(int commitmentId)
        {
            return _commitmentRepo.GetCommitmentById(commitmentId);
        }

        public void UpdateCommitment(UpdateCommitmentDto dto)
        {
            var commitment = _commitmentRepo.GetCommitmentById(dto.CommitmentId);
            if (commitment == null)
                throw new Exception("Commitment not found.");

            commitment.Title = dto.Title;
            commitment.Description = dto.Description;

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