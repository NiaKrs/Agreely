using Agreely.Services.DTO.Requests;
using Agreely.Services.DTO.Responses;

namespace Agreely.Services.Interfaces
{
    public interface ICommitmentService
    {
        int CreateCommitment(CreateCommitmentRequest request);
        int CreateCommitmentVersion(CreateCommitmentVersionRequest request);
        List<ViewCommitmentResponse> GetCommitmentsByGroupId(int groupId);
        ViewCommitmentResponse? GetCommitmentById(int commitmentId);
        void DeleteCommitment(int commitmentId);
        
    }
}