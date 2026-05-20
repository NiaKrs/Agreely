using Agreely.Domain;
using Agreely.Repositories.Interfaces;
using Agreely.Services.DTO.Requests;
using Agreely.Services.Services;
using Moq;

namespace Agreely.UnitTests
{
    public class AuthServiceTests
    {
        private readonly Mock<IUserRepository> _userRepoMock;
        private readonly AuthService _authService;

        public AuthServiceTests()
        {
            _userRepoMock = new Mock<IUserRepository>();
            _authService = new AuthService(_userRepoMock.Object);
        }

        [Fact]
        public void RegisterUser_NewEmail_ReturnsUserId()
        {
            var request = new RegisterRequest { FullName = "Nia", Email = "nia@test.com", Password = "pass123" };
            _userRepoMock.Setup(r => r.GetUserByEmail("nia@test.com")).Returns((User?)null);
            _userRepoMock.Setup(r => r.CreateUser(It.IsAny<User>())).Returns(1);

            var result = _authService.RegisterUser(request);

            Assert.Equal(1, result);
        }

        [Fact]
        public void RegisterUser_EmailAlreadyExists_ThrowsException()
        {
            var request = new RegisterRequest { FullName = "Nia", Email = "nia@test.com", Password = "pass123" };
            _userRepoMock.Setup(r => r.GetUserByEmail("nia@test.com")).Returns(new User { Email = "nia@test.com" });

            var ex = Assert.Throws<Exception>(() => _authService.RegisterUser(request));
            Assert.Equal("An account with this email already exists.", ex.Message);
        }

        [Fact]
        public void LoginUser_CorrectCredentials_ReturnsUser()
        {
            var passwordHash = BCrypt.Net.BCrypt.HashPassword("pass123");
            var request = new LoginRequest { Email = "nia@test.com", Password = "pass123" };
            _userRepoMock.Setup(r => r.GetUserByEmail("nia@test.com")).Returns(new User { Email = "nia@test.com", PasswordHash = passwordHash });

            var result = _authService.LoginUser(request);

            Assert.NotNull(result);
        }

        [Fact]
        public void LoginUser_WrongPassword_ReturnsNull()
        {
            var passwordHash = BCrypt.Net.BCrypt.HashPassword("pass123");
            var request = new LoginRequest { Email = "nia@test.com", Password = "wrongpass" };
            _userRepoMock.Setup(r => r.GetUserByEmail("nia@test.com")).Returns(new User { Email = "nia@test.com", PasswordHash = passwordHash });

            var result = _authService.LoginUser(request);

            Assert.Null(result);
        }

        [Fact]
        public void LoginUser_EmailNotFound_ReturnsNull()
        {
            var request = new LoginRequest { Email = "unknown@test.com", Password = "pass123" };
            _userRepoMock.Setup(r => r.GetUserByEmail("unknown@test.com")).Returns((User?)null);

            var result = _authService.LoginUser(request);

            Assert.Null(result);
        }
    }
}