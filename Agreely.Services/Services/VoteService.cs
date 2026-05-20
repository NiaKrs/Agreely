using Agreely.Domain;
using Agreely.Domain.Enums;
using Agreely.Repositories.Interfaces;
using Agreely.Services.DTO.Requests;
using Agreely.Services.Interfaces;


namespace Agreely.Services.Services
{
    public class VoteService : IVoteService
    {
        private readonly IVoteRepository _voteRepo;

        public VoteService(IVoteRepository voteRepo)
        {
            _voteRepo = voteRepo;
        }

        public void CastOrUpdateVote(CastVoteRequest request)
        {
            var vote = _voteRepo.GetVote(request.CommitmentVersionId, request.UserId);
            if (vote == null)
            {
                _voteRepo.CastVote(new AlignmentVote
                {
                    CommitmentVersionId = request.CommitmentVersionId,
                    UserId = request.UserId,
                    Vote = request.Vote
                });
            }
            else
            {
                _voteRepo.UpdateVote(new AlignmentVote
                {
                    CommitmentVersionId = request.CommitmentVersionId,
                    UserId = request.UserId,
                    Vote = request.Vote
                });
            }
        }

        public VoteValue? GetUserVote(int commitmentVersionId, int userId)
        {
            var vote = _voteRepo.GetVote(commitmentVersionId, userId);
            return vote?.Vote;
        }

    }
}
