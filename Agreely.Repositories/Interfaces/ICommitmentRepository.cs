using Agreely.Repositories.Models;

namespace Agreely.Repositories.Interfaces
{
    public interface ICommitmentRepository
    {
        int CreateCommitment(Commitment commitment);
        List<Commitment> GetCommitmentsByGroupId(int groupId);
        Commitment? GetCommitmentById(int commitmentId);
        void UpdateCommitment(Commitment commitment);
        void DeleteCommitment(int commitmentId);
    }
}