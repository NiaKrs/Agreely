using Agreely.Services.DTO.Requests;
using Agreely.Services.Interfaces;
using Agreely.Services.Services;
using Agreely.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace Agreely.Controllers
{
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
                TempData["Success"] = "You registrated successfully!";
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
        public IActionResult Login(LoginViewModel vm)
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

            HttpContext.Session.SetInt32("UserId", user.UserId);
            HttpContext.Session.SetString("FullName", user.FullName);
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}
