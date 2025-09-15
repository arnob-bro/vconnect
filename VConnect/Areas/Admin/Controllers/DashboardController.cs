using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VConnect.Areas.Admin.Models;
using VConnect.Models;
using VConnect.Models.Events;
using VConnect.Database;
using VConnect.Filters;

namespace VConnect.Areas.Admin.Controllers
{
    [Area("Admin")]
    [AdminOnly]
    public class DashboardController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DashboardController(ApplicationDbContext context)
        {
            _context = context;

        }


        [HttpGet]

        public async Task<IActionResult> Index()
        {
            var model = new AdminDashboardViewModel
            {
                // Stats
                TotalDonationGot = await _context.Donations.SumAsync(d => d.Amount),
                //TotalDonationGave = await _context.Donations.SumAsync(d => d.AmountGiven),
                TotalEvents = await _context.Events.CountAsync(),
                TotalVolunteers = await _context.Users.CountAsync(u => u.Role == "Volunteer"),

                // Last 5 ta Events
                LastEvents = await _context.Events
                                .OrderByDescending(e => e.StartDateTime)
                                .Take(5)
                                .ToListAsync(),

                // Last 5 ta Donations paisi
                LastDonationsGot = await _context.Donations
                                    .OrderByDescending(d => d.CreatedAt)
                                    .Take(5)
                                    .ToListAsync(),

                // Last 5 ta Donations disi
                //LastDonationsGave = await _context.Donations
                //                    .Where(d => d.Type == "Gave")
                //                    .OrderByDescending(d => d.Date)
                //                    .Take(5)
                //                    .ToListAsync(),

                
                
            };

            return View(model);
        }
    }
}
