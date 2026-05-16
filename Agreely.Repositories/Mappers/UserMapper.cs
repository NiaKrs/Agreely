using Agreely.Domain;
using Agreely.Repositories.Entities;

namespace Agreely.Repositories.Mappers
{
    public static class UserMapper
    {
        public static User ToDomain(UserEntity entity) => new User
        {
            UserId = entity.UserId,
            FullName = entity.FullName,
            Email = entity.Email,
            PasswordHash = entity.Password
        };
    }
}