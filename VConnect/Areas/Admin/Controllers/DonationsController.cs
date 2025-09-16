using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VConnect.Models;
using VConnect.Database;
using VConnect.Filters;

namespace VConnect.Areas.Admin.Controllers
{
    [Area("Admin")]
    [AdminOnly]
    public class DonationsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DonationsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var donations = await _context.Donations.ToListAsync();

            var stats = new DonationStats
            {
                TotalDonations = donations.Sum(d => d.Amount),
                TotalDonors = donations.Select(d => d.Email).Distinct().Count(),
                TotalDonationsProvided = await _context.DonationProvided.SumAsync(d => d.Amount),
                AllDonations = donations
            };

            return View(stats);
        }


        public async Task<IActionResult> Details(int id)
        {
            var donation = await _context.Donations
                .FirstOrDefaultAsync(d => d.Id == id);

            if (donation == null)
            {
                return NotFound();
            }

            return View(donation);
        }
    }
}
