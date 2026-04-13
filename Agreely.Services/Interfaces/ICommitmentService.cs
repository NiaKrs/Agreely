using Agreely.Services.DTOs;

namespace Agreely.Services.Interfaces
{
    public interface ICommitmentService
    {
        int CreateCommitment(CreateCommitmentDto dto);
    }
}