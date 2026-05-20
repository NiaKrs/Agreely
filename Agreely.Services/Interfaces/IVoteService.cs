

using Agreely.Domain.Enums;
using Agreely.Services.DTO.Requests;
using Agreely.Services.DTO.Responses;

namespace Agreely.Services.Interfaces
{
    public interface IVoteService
    {
        void CastOrUpdateVote(CastVoteRequest request);
        VoteValue? GetUserVote(int commitmentVersionId, int userId);
    }
}
