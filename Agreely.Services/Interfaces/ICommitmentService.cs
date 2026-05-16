using Agreely.Services.DTO.Requests;
using Agreely.Services.DTO.Responses;

namespace Agreely.Services.Interfaces
{
    public interface ICommitmentService
    {
        int CreateCommitment(CreateCommitmentRequest request);
        List<ViewCommitmentResponse> GetCommitmentsByGroupId(int groupId);
        ViewCommitmentResponse? GetCommitmentById(int commitmentId);
        void UpdateCommitment(UpdateCommitmentRequest request);
        void DeleteCommitment(int commitmentId);
        
    }
}