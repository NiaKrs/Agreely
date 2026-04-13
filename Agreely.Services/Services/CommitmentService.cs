using Agreely.Repositories.Interfaces;
using Agreely.Repositories.Models;
using Agreely.Services.DTOs;
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
    }
}