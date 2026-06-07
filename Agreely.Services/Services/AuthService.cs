using Agreely.Repositories.Interfaces;
using Agreely.Services.DTO.Requests;
using Agreely.Services.Interfaces;
using Agreely.Domain;

namespace Agreely.Services.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepo;

        public AuthService(IUserRepository userRepo)
        {
            _userRepo = userRepo;
        }

        public int RegisterUser(RegisterRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.FullName))
                throw new Exception("Full name is required.");

            if (string.IsNullOrWhiteSpace(request.Email) || !request.Email.Contains("@"))
                throw new Exception("A valid email address is required.");

            if (string.IsNullOrWhiteSpace(request.Password))
                throw new Exception("Password is required.");

            var existingUser = _userRepo.GetUserByEmail(request.Email);
            if (existingUser != null)
                throw new Exception("An account with this email already exists.");

            var passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);
            var user = new User
            {
                FullName = request.FullName,
                Email = request.Email,
                PasswordHash = passwordHash
            };

            return _userRepo.CreateUser(user);
        }

        public User? LoginUser(LoginRequest request)
        {
           var user = _userRepo.GetUserByEmail(request.Email);
            if (user != null && BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            {
                return user;
            }
            return null;
        }
    }
}
