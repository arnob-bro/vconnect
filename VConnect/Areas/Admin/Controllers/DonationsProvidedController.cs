using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VConnect.Database;
using VConnect.Filters;
using VConnect.Models;

namespace VConnect.Areas.Admin.Controllers
{
    [Area("Admin")]
    [AdminOnly]
    public class DonationsProvidedController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DonationsProvidedController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Admin/DonationsProvided
        public async Task<IActionResult> Index()
        {
            var donationsProvided = await _context.DonationProvided.ToListAsync();

            var stats = new DonationStats
            {
                TotalDonationsProvided = donationsProvided.Sum(d => d.Amount),
                // If you also want to show received donations for comparison
                TotalDonations = await _context.Donations.SumAsync(d => d.Amount),
                TotalDonors = await _context.DonationProvided.Select(d => d.Id).Distinct().CountAsync(),
                AllDonationsProvided = donationsProvided
            };

            return View(stats);
        }


        // GET: Admin/DonationsProvided/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var donationProvided = await _context.DonationProvided
                .FirstOrDefaultAsync(dp => dp.Id == id);

            if (donationProvided == null) return NotFound();

            return View(donationProvided);
        }

        // GET: Admin/DonationsProvided/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/DonationsProvided/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(DonationProvided donationProvided)
        {
            if (ModelState.IsValid)
            {
                donationProvided.ProvidedAt = DateTime.UtcNow;
                _context.Add(donationProvided);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(donationProvided);
        }

        // GET: Admin/DonationsProvided/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var donationProvided = await _context.DonationProvided
                .FirstOrDefaultAsync(dp => dp.Id == id);

            if (donationProvided == null) return NotFound();

            return View(donationProvided);
        }

        // POST: Admin/DonationsProvided/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var donationProvided = await _context.DonationProvided.FindAsync(id);
            if (donationProvided != null)
            {
                _context.DonationProvided.Remove(donationProvided);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
