using Microsoft.AspNetCore.Mvc;
using VConnect.Models;
using VConnect.Services;

using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

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
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View("~/Views/Account1/Login.cshtml", model);

            var user = await _userService.AuthenticateUserAsync(model);
            if (user == null)
            {
                ModelState.AddModelError("", "Invalid email or password.");
                return View("~/Views/Account1/Login.cshtml", model);
            }

            var fullName = $"{user.FirstName} {user.LastName}".Trim();
            if (string.IsNullOrWhiteSpace(fullName)) fullName = user.Email;

            // Use a default avatar for now. Later you can set this to a real URL.
            var avatarUrl = Url.Content("~/images/avatar-default.png");

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, fullName),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim("avatar_url", avatarUrl) // navbar will read this
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            var authProps = new AuthenticationProperties
            {
                IsPersistent = model.RememberMe,
                ExpiresUtc = DateTimeOffset.UtcNow.AddDays(model.RememberMe ? 30 : 1)
            };

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, authProps);
            return RedirectToAction("Index", "Home");
        }

        // POST: /Account/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
                return View("~/Views/Account1/Register.cshtml", model);

            var user = await _userService.RegisterUserAsync(model);
            if (user == null)
            {
                ModelState.AddModelError("", "Email already exists.");
                return View("~/Views/Account1/Register.cshtml", model);
            }

            return RedirectToAction("Login", "Account");
        }

        // POST: /Account/Logout
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }

        // Optional
        [HttpGet]
        public IActionResult Profile()
        {
            return RedirectToAction("Index", "ProfileDetails");
        }
    }
}
