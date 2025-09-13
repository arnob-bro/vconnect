using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VConnect.Models.Enums;
using VConnect.Models.Events;
using VConnect.Services;

namespace VConnect.Controllers
{
    public class EventController : Controller
    {
        private readonly IEventService _service;

        public EventController(IEventService service)
        {
            _service = service;
        }

        // Volunteer views
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            var events = await _service.GetAllAsync();
            return View(events);
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Details(int id)
        {
            var evt = await _service.GetByIdAsync(id);
            if (evt == null) return NotFound();
            return View(evt);
        }

        [HttpPost]
        [Authorize] // must be logged in to apply
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Apply(int eventId, int roleId)
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdStr, out var userId) || userId <= 0)
            {
                TempData["Error"] = "User not recognized.";
                return RedirectToAction("Details", new { id = eventId });
            }

            var application = await _service.ApplyAsync(eventId, roleId, userId);
            if (application == null)
            {
                TempData["Error"] = "Could not apply. Capacity may be full or you already applied.";
                return RedirectToAction("Details", new { id = eventId });
            }

            TempData["Success"] = "Application submitted!";
            return RedirectToAction("Details", new { id = eventId });
        }

        // ADMIN AREA
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public IActionResult Create() => View();

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Event evt)
        {
            if (!ModelState.IsValid) return View(evt);
            await _service.CreateAsync(evt);
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id)
        {
            var evt = await _service.GetByIdAsync(id);
            if (evt == null) return NotFound();
            return View(evt);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Event evt)
        {
            if (!ModelState.IsValid) return View(evt);
            var updated = await _service.UpdateAsync(evt);
            if (updated == null) return NotFound();
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            await _service.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SetApplicationStatus(int appId, ApplicationStatus status, int eventId)
        {
            var ok = await _service.UpdateApplicationStatusAsync(appId, status);
            TempData[ok ? "Success" : "Error"] = ok ? "Status updated." : "Could not update status.";
            return RedirectToAction("Details", new { id = eventId });
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RecordParticipation(int eventId, int userId, int roleId, int hours)
        {
            await _service.RecordParticipationAsync(eventId, userId, roleId, hours);
            TempData["Success"] = "Participation recorded.";
            return RedirectToAction("Details", new { id = eventId });
        }
    }
}
