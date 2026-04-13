using Agreely.Repositories.Models;
using Agreely.Services.DTO;

namespace Agreely.Services.Interfaces
{
    public interface ICommitmentService
    {
        int CreateCommitment(CreateCommitmentDto dto);
        List<Commitment> GetCommitmentsByGroupId(int groupId);
        Commitment? GetCommitmentById(int commitmentId);
        void UpdateCommitment(UpdateCommitmentDto dto);
        void DeleteCommitment(int commitmentId);
        
    }
}