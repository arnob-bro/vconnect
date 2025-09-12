using VConnect.Models;   // ← this is mandatory
using VConnect.Database;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VConnect.Services
{
    public class DonationService : IDonationService
    {
        private readonly ApplicationDbContext _context;

        public DonationService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Donation> CreateDonationAsync(Donation donation)
        {
            donation.CreatedAt = DateTime.UtcNow;
            _context.Donations.Add(donation);
            await _context.SaveChangesAsync();
            return donation;
        }

        public async Task<List<Donation>> GetAllDonationsAsync()
        {
            return await _context.Donations
                .OrderByDescending(d => d.CreatedAt)
                .ToListAsync();
        }

        public async Task<Donation> GetDonationByIdAsync(int id)
        {
            return await _context.Donations.FindAsync(id);
        }

        public async Task<bool> UpdateDonationStatusAsync(int id, DonationStatus status)
        {
            var donation = await _context.Donations.FindAsync(id);
            if (donation == null) return false;

            donation.Status = status;
            donation.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<Donation>> GetRecentDonationsAsync(int count)
        {
            return await _context.Donations
                .OrderByDescending(d => d.CreatedAt)
                .Take(count)
                .ToListAsync();
        }

        public async Task<DonationStats> GetDonationStatsAsync()
        {
            var donations = await _context.Donations.ToListAsync();
            var monthly = donations.Where(d => d.CreatedAt >= DateTime.UtcNow.AddMonths(-1));

            var stats = new DonationStats
            {
                TotalDonations = donations.Sum(d => d.Amount),
                TotalDonors = donations.Select(d => d.Email).Distinct().Count(),
                MonthlyDonations = monthly.Sum(d => d.Amount),
                MonthlyDonors = monthly.Select(d => d.Email).Distinct().Count(),
                RecentDonations = donations.OrderByDescending(d => d.CreatedAt)
                                           .Take(5)
                                           .Select(d => new RecentDonation
                                           {
                                               DonorName = d.DonorName,
                                               DonorInitials = string.Join("", d.DonorName.Split(' ').Select(n => n[0])),
                                               Amount = d.Amount,
                                               Date = d.CreatedAt,
                                               IsAnonymous = d.IsAnonymous
                                           }).ToList(),
                DonationsByMethod = donations.GroupBy(d => d.PaymentMethod.ToString())
                                             .ToDictionary(g => g.Key, g => g.Sum(d => d.Amount))
            };

            return stats;
        }
    }
}
