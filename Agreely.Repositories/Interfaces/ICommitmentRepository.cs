using Agreely.Domain;
using Agreely.Domain.Enums;

namespace Agreely.Repositories.Interfaces
{
    public interface ICommitmentRepository
    {
        int InsertCommitment(Commitment commitment);
        int CreateCommitmentVersion(CommitmentVersion version);
        CommitmentVersion? GetCurrentVersion(int commitmentId);
        void DeactivatePreviousVersions(int commitmentId);
        void UpdateCommitmentStatus(int commitmentId, CommitmentStatus status);
        List<Commitment> GetCommitmentsByGroupId(int groupId);
        Commitment? GetCommitmentById(int commitmentId);
        void DeleteVotesByCommitmentId(int commitmentId);
        void DeleteVersionsByCommitmentId(int commitmentId);
        void DeleteCommitment(int commitmentId);
    }
}