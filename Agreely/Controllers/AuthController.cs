using Agreely.Services.DTO.Requests;
using Agreely.Services.Interfaces;
using Agreely.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Agreely.Controllers
{
    [AllowAnonymous]
    public class AuthController : BaseController
    {
        private readonly IAuthService _authService;

        public AuthController (IAuthService authService)
        {
            _authService = authService;
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View(new RegisterViewModel());
        }

        [HttpPost]
        public IActionResult Register(RegisterViewModel vm)
        {
            if (!ModelState.IsValid)
                return View(vm);
            try
            {
                var request = new RegisterRequest
                {
                    FullName = vm.FullName,
                    Email = vm.Email,
                    Password = vm.Password
                };
                _authService.RegisterUser(request);
                TempData["Success"] = "You registered successfully!";
                return RedirectToAction("Login");
            }
            catch (Exception ex)
            {
                ViewData["Error"] = ex.Message;
                return View(vm);
            }
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View(new LoginViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel vm)
        {
            if (!ModelState.IsValid)
                return View(vm);
            var request = new LoginRequest
            {
                Email = vm.Email,
                Password = vm.Password
            };

            var user = _authService.LoginUser(request);
            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Invalid email or password.");
                return View(vm);
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                new Claim(ClaimTypes.Name, user.FullName),
                new Claim(ClaimTypes.Email, user.Email)
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

            return RedirectToAction("MyGroups", "Group");
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }
    }
}
