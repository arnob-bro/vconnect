using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
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
            // you set this claim in AccountController during login
            var id = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return int.Parse(id!);
        }

        // GET: /ProfileDetails
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var userId = CurrentUserId();
            var model = await _service.EnsureForUserAsync(userId);
            return View(model); // Views/ProfileDetails/Index.cshtml
        }

        // GET: /ProfileDetails/Edit
        [HttpGet]
        public async Task<IActionResult> Edit()
        {
            var userId = CurrentUserId();
            var model = await _service.GetByUserIdAsync(userId);
            if (model == null) return RedirectToAction(nameof(Index));
            return View(model); // Views/ProfileDetails/Edit.cshtml
        }

        // POST: /ProfileDetails/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ProfileDetails model)
        {
            // Your file input is bound to ProfilePictureUrl (string). We’ll read the real file here:
            var upload = Request.Form.Files.FirstOrDefault();

            // make sure it is tied to the current user
            model.UserId = CurrentUserId();

            // if the binder added a model error because of the file/string mismatch, clear it
            if (ModelState.ContainsKey(nameof(ProfileDetails.ProfilePictureUrl)))
            {
                ModelState[nameof(ProfileDetails.ProfilePictureUrl)]!.Errors.Clear();
            }

            if (!ModelState.IsValid)
                return View(model);

            await _service.UpdateAsync(model, upload, _env.WebRootPath);

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
            return View(model); // Views/ProfileDetails/Delete.cshtml (your current delete page shows form; adapt if needed)
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
    }
}
