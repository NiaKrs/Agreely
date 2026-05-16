using Agreely.Domain;

namespace Agreely.Repositories.Interfaces
{
    public interface IUserRepository
    {
        int CreateUser(User user);
        User? GetUserByEmail(string email);
    }
}
