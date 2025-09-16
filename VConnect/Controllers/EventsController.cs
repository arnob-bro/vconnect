using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using VConnect.Database;
using VConnect.Models.Enums;
using VConnect.Models.Events;
using VConnect.Services;

namespace VConnect.Controllers
{
    public class EventsController : Controller
    {
        private readonly IEventService _service;
        private readonly ApplicationDbContext _context;

        public EventsController(IEventService service, ApplicationDbContext context)
        {
            _service = service;
            _context = context;
        }

        // PUBLIC LIST (with filters & paging)
        // /Events?search=&category=&status=&month=&page=1
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Index(
            string? search,
            string? category,
            EventStatus? status,
            string? month,
            int page = 1)
        {
            // Service should filter + optionally page
            var events = await _service.GetFilteredAsync(search, category, status, month, page);

            // Keep selected filter values for the view
            ViewBag.Search = search;
            ViewBag.Category = category;
            ViewBag.Status = status;
            ViewBag.Month = month;
            ViewBag.Page = page;

            return View(events);
        }

        // PUBLIC DETAILS
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Details(int id)
        {
            // Load the event including applications and users
            var evt = await _service.GetByIdAsync(id);
            if (evt == null) return NotFound();

            // Get current user info
            var currentUserEmail = User.FindFirstValue(ClaimTypes.Email);
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == currentUserEmail);

            if (user != null)
            {
                ViewBag.HoursVolunteered = user.HoursVolunteered;

                // Has the user already applied?
                ViewBag.HasApplied = evt.Applications?.Any(a => a.UserId == user.Id) ?? false;

                // Can the user apply?
                ViewBag.CanApply = evt.Status == EventStatus.Active &&
                                   (evt.Applications?.Count(a => a.Status == ApplicationStatus.Accepted) ?? 0) < evt.Capacity;

                // Pass current user ID to view to check creator
                ViewBag.CurrentUserId = user.Id;
            }

            return View(evt);
        }

        // VOLUNTEER APPLY
        [HttpPost]
        [Authorize] // must be logged in to apply
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Apply(int eventId)
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdStr, out var userId) || userId <= 0)
            {
                TempData["Error"] = "User not recognized.";
                return RedirectToAction("Details", new { id = eventId });
            }

            var application = await _service.ApplyAsync(eventId, userId);
            if (application == null)
            {
                TempData["Error"] = "Could not apply. Capacity may be full or you already applied.";
                return RedirectToAction("Details", new { id = eventId });
            }

            TempData["Success"] = "Application submitted!";
            return RedirectToAction("Details", new { id = eventId });
        }

        // =========================
        // ADMIN AREA
        // =========================

        [HttpGet]
        //[Authorize(Roles = "Admin")]
        public IActionResult Create() => View();

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Event evt)
        {
            

            if (ModelState.IsValid)
            {
                var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (!int.TryParse(userIdStr, out var userId) || userId <= 0)
                {
                    TempData["Error"] = "User not recognized.";
                    return RedirectToAction(nameof(Index));
                }

                evt.CreatedById = userId;
                evt.StartDateTime = DateTime.SpecifyKind(evt.StartDateTime, DateTimeKind.Utc);
                evt.EndDateTime = DateTime.SpecifyKind(evt.EndDateTime, DateTimeKind.Utc);
                evt.CreatedAt = DateTime.UtcNow;
                evt.UpdatedAt = DateTime.UtcNow;

                await _service.CreateAsync(evt);
                return RedirectToAction(nameof(Index));
            }
            return View(evt);
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangeApplicationStatus(int eventId, int applicationId, ApplicationStatus newStatus)
        {
            var ev = await _context.Events
                .Include(e => e.Applications)
                .FirstOrDefaultAsync(e => e.EventId == eventId);

            if (ev == null)
                return NotFound();

            var app = ev.Applications?.FirstOrDefault(a => a.EventApplicationId == applicationId);
            if (app == null)
                return NotFound();

            app.Status = newStatus;
            await _context.SaveChangesAsync();

            return RedirectToAction("Details", new { id = eventId });
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangeStatus(int eventId, EventStatus newStatus)
        {
            // Load event with applications and participations
            var ev = await _context.Events
                .Include(e => e.Applications)
                .ThenInclude(a => a.User) 
                .Include(e => e.Participations)
                .FirstOrDefaultAsync(e => e.EventId == eventId);

            if (ev == null)
                return NotFound();

            
            var currentUserIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(currentUserIdStr, out var currentUserId))
                return Forbid();

            
            if (ev.CreatedById != currentUserId)
                return Forbid();

            
            ev.Status = newStatus;

            
            if (newStatus == EventStatus.Completed && ev.Applications != null)
            {
                
                var durationHours = (int)Math.Round((ev.EndDateTime - ev.StartDateTime).TotalHours);

                foreach (var app in ev.Applications.Where(a => a.Status == ApplicationStatus.Accepted))
                {
                    
                    var participation = ev.Participations?.FirstOrDefault(p => p.UserId == app.UserId);

                    if (participation != null)
                    {
                        
                        participation.HoursContributed += durationHours;
                    }
                    else
                    {
                        
                        ev.Participations ??= new List<Participation>();
                        ev.Participations.Add(new Participation
                        {
                            EventId = ev.EventId,
                            UserId = app.UserId,
                            HoursContributed = durationHours
                        });
                    }
                }
            }

            ev.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return RedirectToAction("Details", new { id = eventId });
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
        public async Task<IActionResult> RecordParticipation(int eventId, int userId, int hours)
        {
            await _service.RecordParticipationAsync(eventId, userId, hours);
            TempData["Success"] = "Participation recorded.";
            return RedirectToAction("Details", new { id = eventId });
        }
    }
}
