using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VConnect.Models;
using VConnect.Services;

namespace VConnect.Controllers
{
    [Authorize]
    public class ProfileDetailsController : Controller
    {
        private readonly IProfileDetailsService _service;
        private readonly IWebHostEnvironment _env;

        public ProfileDetailsController(IProfileDetailsService service, IWebHostEnvironment env)
        {
            _service = service;
            _env = env;
        }

        private int CurrentUserId()
        {
            var id = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return int.Parse(id!);
        }

        // GET: /ProfileDetails
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var userId = CurrentUserId();
            var model = await _service.EnsureForUserAsync(userId);
            return View(model);
        }

        // GET: /ProfileDetails/Edit
        [HttpGet]
        public async Task<IActionResult> Edit()
        {
            var userId = CurrentUserId();
            var model = await _service.GetByUserIdAsync(userId);
            if (model == null) return RedirectToAction(nameof(Index));
            return View(model);
        }

        // POST: /ProfileDetails/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ProfileDetails model)
        {
            // Get the uploaded file using the correct name from the form
            var upload = Request.Form.Files["ProfilePictureFile"];

            model.UserId = CurrentUserId();

            if (!ModelState.IsValid)
                return View(model);

            // Update the profile (this will handle the file upload and set ProfilePictureUrl)
            await _service.UpdateAsync(model, upload, _env.WebRootPath);

            // Get the updated profile to get the new ProfilePictureUrl
            var updatedProfile = await _service.GetByUserIdAsync(model.UserId);

            // Update user claims with new avatar
            if (updatedProfile != null)
            {
                await UpdateUserClaims(updatedProfile.ProfilePictureUrl);
            }

            TempData["ProfileSaved"] = true;
            return RedirectToAction(nameof(Index));
        }

        // GET: /ProfileDetails/Delete
        [HttpGet]
        public async Task<IActionResult> Delete()
        {
            var userId = CurrentUserId();
            var model = await _service.GetByUserIdAsync(userId);
            if (model == null) return RedirectToAction(nameof(Index));
            return View(model);
        }

        // POST: /ProfileDetails/Delete
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed()
        {
            var userId = CurrentUserId();
            await _service.DeleteAsync(userId);
            return RedirectToAction("Index", "Home");
        }

        // Update user claims with new profile picture URL
        private async Task UpdateUserClaims(string? profilePictureUrl)
        {
            var identity = (ClaimsIdentity)User.Identity!;

            // Remove old avatar claims
            var oldClaims = identity.FindAll(c => c.Type == "ProfilePictureUrl" || c.Type == "avatar_url").ToList();
            foreach (var claim in oldClaims)
            {
                identity.RemoveClaim(claim);
            }

            // Add new avatar claim if exists
            if (!string.IsNullOrWhiteSpace(profilePictureUrl))
            {
                identity.AddClaim(new Claim("ProfilePictureUrl", profilePictureUrl));
            }

            // Update the authentication cookie
            var principal = new ClaimsPrincipal(identity);
            await HttpContext.SignInAsync(principal);
        }
    }
}