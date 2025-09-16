using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VConnect.Areas.Admin.Models;
using VConnect.Database;
using VConnect.Filters;
using VConnect.Models;

namespace VConnect.Areas.Admin.Controllers
{
    [Area("Admin")]
    [AdminOnly]
    public class UsersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public UsersController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Admin/Users
        public async Task<IActionResult> Index()
        {
            // Get all users (or only volunteers)
            var users = await _context.Users
                            .ToListAsync();

            return View(users);
        }

        // GET: Admin/Users/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/Users/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ApplicationUser user)
        {
            if (ModelState.IsValid)
            {
                // Optional: set default role if empty
                if (string.IsNullOrEmpty(user.Role))
                    user.Role = "Admin";

                var hashed = BCrypt.Net.BCrypt.HashPassword(user.Password);
                user.Password = hashed;

                _context.Users.Add(user);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(user);
        }

        

    }
}
