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
    }
}
