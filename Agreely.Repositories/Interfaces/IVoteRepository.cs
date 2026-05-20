using Agreely.Domain;

namespace Agreely.Repositories.Interfaces
{
    public interface IVoteRepository
    {
        void CastVote(AlignmentVote vote);
        AlignmentVote? GetVote(int commitmentVersionId, int userId);
        void UpdateVote(AlignmentVote newVote);
        List<AlignmentVote> GetVotesByVersion(int commitmentVersionId);
    }
}
