using VConnect.Models;
using VConnect.Database;
using Microsoft.EntityFrameworkCore;

namespace VConnect.Services
{
    public class DonationService
    {
        private readonly ApplicationDbContext _context;

        public DonationService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Donation> ProcessDonationAsync(DonationViewModel model)
        {
            var donation = new Donation
            {
                DonorName = model.DonorName,
                Email = model.Email,
                Phone = model.Phone,
                Amount = model.Amount,
                Message = model.Message,
                PaymentMethod = model.PaymentMethod,
                IsAnonymous = model.IsAnonymous,
                DonationDate = DateTime.Now,
                TransactionId = GenerateTransactionId(),
                Status = DonationStatus.Pending
            };

            // TODO: integrate real payment gateway here
            donation.Status = DonationStatus.Completed;

            _context.Donations.Add(donation);
            await _context.SaveChangesAsync();

            return donation;
        }

        public async Task<DonationStats> GetDonationStatsAsync()
        {
            var totalDonations = await _context.Donations.SumAsync(d => d.Amount);
            var totalDonors = await _context.Donations.Select(d => d.Email).Distinct().CountAsync();

            var monthlyDonations = await _context.Donations
                .Where(d => d.DonationDate.Month == DateTime.Now.Month &&
                            d.DonationDate.Year == DateTime.Now.Year)
                .SumAsync(d => d.Amount);

            var monthlyDonors = await _context.Donations
                .Where(d => d.DonationDate.Month == DateTime.Now.Month &&
                            d.DonationDate.Year == DateTime.Now.Year)
                .Select(d => d.Email).Distinct().CountAsync();

            // Fetch recent donations into memory first
            var recentDonationsData = await _context.Donations
                .OrderByDescending(d => d.DonationDate)
                .Take(5)
                .ToListAsync();

            var recentDonations = recentDonationsData.Select(d => new RecentDonation
            {
                DonorInitials = string.IsNullOrEmpty(d.DonorName) ? "NA" :
                                new string(d.DonorName.Split(' ')
                                .Where(w => !string.IsNullOrEmpty(w))
                                .Select(w => w[0]).ToArray()).ToUpper(),
                DonorName = d.IsAnonymous ? "Anonymous" : d.DonorName,
                Amount = d.Amount,
                Date = d.DonationDate,
                IsAnonymous = d.IsAnonymous
            }).ToList();

            // Fetch donations grouped by payment method into memory
            var donationsByMethodData = await _context.Donations
                .GroupBy(d => d.PaymentMethod)
                .ToListAsync();

            var donationsByMethod = donationsByMethodData
                .ToDictionary(g => g.Key, g => g.Sum(d => d.Amount));

            return new DonationStats
            {
                TotalDonations = totalDonations,
                TotalDonors = totalDonors,
                MonthlyDonations = monthlyDonations,
                MonthlyDonors = monthlyDonors,
                RecentDonations = recentDonations,
                DonationsByMethod = donationsByMethod
            };
        }

        private string GenerateTransactionId()
        {
            return "VC" + DateTime.Now.ToString("yyyyMMddHHmmss") +
                   new Random().Next(1000, 9999);
        }
    }
}
