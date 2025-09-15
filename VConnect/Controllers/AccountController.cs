using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VConnect.Models;
using VConnect.Services;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Threading.Tasks;
using System.Collections.Generic;

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
        [AllowAnonymous]
        public IActionResult Login()
        {
            return View("~/Views/Account1/Login.cshtml");
        }

        // GET: /Account/Register
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Register()
        {
            return View("~/Views/Account1/Register.cshtml");
        }

        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View("~/Views/Account1/Login.cshtml", model);

            var user = await _userService.AuthenticateUserAsync(model);
            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Invalid email or password.");
                return View("~/Views/Account1/Login.cshtml", model);
            }

            var fullName = $"{user.FirstName} {user.LastName}".Trim();
            if (string.IsNullOrWhiteSpace(fullName)) fullName = user.Email;

            var avatarUrl = Url.Content("~/images/avatar-default.png");

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, fullName),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim("avatar_url", avatarUrl)
            };

            // include Role claim if available (e.g., "Admin" or "Volunteer")
            if (!string.IsNullOrWhiteSpace(user.Role))
            {
                claims.Add(new Claim(ClaimTypes.Role, user.Role));
            }

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            var authProps = new AuthenticationProperties
            {
                IsPersistent = model.RememberMe,
                ExpiresUtc = System.DateTimeOffset.UtcNow.AddDays(model.RememberMe ? 30 : 1)
            };

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, authProps);

            if (!string.IsNullOrWhiteSpace(user.Role) && user.Role == "Admin")
            {
                return RedirectToAction("Index", "Dashboard", new { area = "Admin" });
            }


            return RedirectToAction("Index", "Home");
        }

        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
                return View("~/Views/Account1/Register.cshtml", model);

            var user = await _userService.RegisterUserAsync(model);
            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Email already exists.");
                return View("~/Views/Account1/Register.cshtml", model);
            }

            // after successful registration, send to login
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

        // Optional profile entry point
        [HttpGet]
        [Authorize]
        public IActionResult Profile()
        {
            return RedirectToAction("Index", "ProfileDetails");
        }
    }
}
