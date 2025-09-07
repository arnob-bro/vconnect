using Microsoft.AspNetCore.Mvc;
using VConnect.Models;
using VConnect.Services;

namespace VConnect.Controllers
{
    public class AccountController : Controller
    {
        private readonly IUserService _userService;

        public AccountController(IUserService userService)
        {
            _userService = userService;
        }

        // GET: /Account/Login
        [HttpGet]
        public IActionResult Login()
        {
            return View("~/Views/Account1/Login.cshtml");
        }

        // GET: /Account/Register
        [HttpGet]
        public IActionResult Register()
        {
            return View("~/Views/Account1/Register.cshtml");
        }

        // POST: /Account/Login
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View("~/Views/Account1/Login.cshtml", model);

            var user = await _userService.AuthenticateUserAsync(model);
            if (user != null)
            {
                // TODO: Set authentication cookie/session
                return RedirectToAction("Index", "Home");
            }

            ModelState.AddModelError("", "Invalid email or password.");
            return View("~/Views/Account1/Login.cshtml", model);
        }

        // POST: /Account/Register
        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
                return View("~/Views/Account1/Register.cshtml", model);

            var user = await _userService.RegisterUserAsync(model);
            if (user != null)
            {
                // Registration successful, redirect to login
                return RedirectToAction("Login", "Account");
            }

            ModelState.AddModelError("", "Email already exists.");
            return View("~/Views/Account1/Register.cshtml", model);
        }
    }
}
