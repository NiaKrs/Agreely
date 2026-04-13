using Agreely.Services.DTO;

namespace Agreely.Services.Interfaces
{
    public interface ICommitmentService
    {
        int CreateCommitment(CreateCommitmentDto dto);
    }
}