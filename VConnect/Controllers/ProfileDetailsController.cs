using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using VConnect.Models;
using VConnect.Services;

namespace VConnect.Controllers
{
    [Authorize]
    public class ProfileDetailsController : Controller
    {
        private readonly IProfileDetailsService _profileService;

        public ProfileDetailsController(IProfileDetailsService profileService)
        {
            _profileService = profileService;
        }

        private int GetCurrentUserId()
        {
            return int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        }

        // GET: /ProfileDetails
        public async Task<IActionResult> Index()
        {
            var userId = GetCurrentUserId();
            var profile = await _profileService.GetByUserIdAsync(userId);

            if (profile == null)
                return RedirectToAction("Create"); // if you want a create page later

            return View(profile);
        }

        // GET: /ProfileDetails/Edit
        public async Task<IActionResult> Edit()
        {
            var userId = GetCurrentUserId();
            var profile = await _profileService.GetByUserIdAsync(userId);

            if (profile == null) return NotFound();
            return View(profile);
        }

        // POST: /ProfileDetails/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ProfileDetails model)
        {
            if (!ModelState.IsValid) return View(model);

            model.UserId = GetCurrentUserId(); // ensure correct linkage
            await _profileService.UpdateAsync(model);

            TempData["ProfileSaved"] = true;
            return RedirectToAction(nameof(Index));
        }

        // GET: /ProfileDetails/Delete
        public async Task<IActionResult> Delete()
        {
            var userId = GetCurrentUserId();
            var profile = await _profileService.GetByUserIdAsync(userId);

            if (profile == null) return NotFound();
            return View(profile);
        }

        // POST: /ProfileDetails/Delete
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed()
        {
            var userId = GetCurrentUserId();
            await _profileService.DeleteAsync(userId);

            return RedirectToAction("Index", "Home");
        }
    }
}
