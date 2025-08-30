using Microsoft.AspNetCore.Mvc;
using VConnect.Models;
using System.Diagnostics;

namespace VConnect.Controllers
{
    public class AccountController : Controller
    {
        // GET: /Account/Login
        public IActionResult Login()
        {
            return View("~/Views/Account1/Login.cshtml");
        }

        // GET: /Account/Register
        public IActionResult Register()
        {
            return View("~/Views/Account1/Register.cshtml");
        }

        // POST: /Account/Login
        [HttpPost]
        public IActionResult Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Add authentication logic here
                // For now, just redirect to home
                return RedirectToAction("Index", "Home");
            }
            return View("~/Views/Account1/Login.cshtml", model);
        }

        // POST: /Account/Register
        [HttpPost]
        public IActionResult Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Add registration logic here
                // For now, just redirect to login
                return RedirectToAction("Login", "Account");
            }
            return View("~/Views/Account1/Register.cshtml", model);
        }
    }
}