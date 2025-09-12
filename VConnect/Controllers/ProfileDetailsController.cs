using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using VConnect.Models;

namespace VConnect.Controllers
{
    [Authorize]
    public class ProfileDetailsController : Controller
    {
        // Later: inject ApplicationDbContext or UserManager here
        // private readonly ApplicationDbContext _context;
        // public ProfileDetailsController(ApplicationDbContext context) { _context = context; }

        // GET: /ProfileDetails
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            // TODO: fetch current user's profile from DB
            // var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            // var profile = await _context.ProfileDetails.FindAsync(userId);

            var model = new ProfileDetails
            {
                FirstName = "Demo",
                LastName = "User",
                Email = User.Identity?.Name ?? "demo@example.com",
                City = "Dhaka",
                Address = "Placeholder Street",
                TotalVolunteerHours = 12,
                EventsParticipated = 3
            };

            return View(model);
        }

        // GET: /ProfileDetails/Edit
        [HttpGet]
        public async Task<IActionResult> Edit()
        {
            // TODO: fetch current user's profile from DB
            var model = new ProfileDetails();
            return View(model);
        }

        // POST: /ProfileDetails/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ProfileDetails model)
        {
            if (!ModelState.IsValid)
                return View(model);

            // TODO: update profile in DB
            // _context.Update(model);
            // await _context.SaveChangesAsync();

            TempData["ProfileSaved"] = true;
            return RedirectToAction(nameof(Index));
        }

        // GET: /ProfileDetails/Delete
        [HttpGet]
        public async Task<IActionResult> Delete()
        {
            // TODO: fetch current user's profile for confirmation
            var model = new ProfileDetails();
            return View(model);
        }

        // POST: /ProfileDetails/Delete
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed()
        {
            // TODO: delete profile from DB
            // await _context.SaveChangesAsync();

            return RedirectToAction("Index", "Home");
        }
    }
}
