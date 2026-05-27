using Agreely.Domain;
using Agreely.Services.DTO.Requests;


namespace Agreely.Services.Interfaces
{
    public interface IAuthService
    {
        int RegisterUser(RegisterRequest request);
        User? LoginUser(LoginRequest request);
    }
}