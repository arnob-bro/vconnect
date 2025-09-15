using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using VConnect.Areas.Admin.Models;
using VConnect.Database;
using VConnect.Filters;
using VConnect.Models.Enums;
using VConnect.Models.Events;

namespace VConnect.Areas.Admin.Controllers
{
    [Area("Admin")]
    [AdminOnly]
    public class EventsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public EventsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /Admin/Events
        public async Task<IActionResult> Index()
        {
            var allEvents = await _context.Events
                                          .OrderByDescending(e => e.CreatedAt)
                                          .ToListAsync();

            // Pass enum values for the dropdown
            ViewBag.EventStatuses = Enum.GetValues(typeof(EventStatus))
                                .Cast<EventStatus>()
                                .Select(s => new SelectListItem
                                {
                                    Text = s.ToString(),
                                    Value = s.ToString()
                                })
                                .ToList();

            var model = new AdminEventViewModel
            {
                AllEvents = allEvents,
                TotalEvents = allEvents.Count
            };

            return View(model);
        }

        // GET: /Admin/Events/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: /Admin/Events/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Event ev)
        {
            if (ModelState.IsValid)
            {
                ev.StartDateTime = DateTime.SpecifyKind(ev.StartDateTime, DateTimeKind.Utc);
                ev.EndDateTime = DateTime.SpecifyKind(ev.EndDateTime, DateTimeKind.Utc);
                ev.CreatedAt = DateTime.UtcNow;
                ev.UpdatedAt = DateTime.UtcNow;

                _context.Events.Add(ev);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(ev);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangeStatus(int eventId, EventStatus newStatus)
        {
            var ev = await _context.Events.FirstOrDefaultAsync(e => e.EventId == eventId);
            if (ev == null)
            {
                return NotFound();
            }

            ev.Status = newStatus;

            try
            {
                await _context.SaveChangesAsync();
                TempData["Success"] = "Event status updated successfully.";
            }
            catch (DbUpdateException)
            {
                TempData["Error"] = "Failed to update event status.";
            }

            return RedirectToAction("Index");
        }
    }
}
