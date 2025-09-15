using VConnect.Models;   // ← this is mandatory
using VConnect.Database;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Buffers;

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
            switch (donation.PaymentMethod)
            {
                case "bKash":
                    donation.NagadNumber = "";
                    donation.BankName = "";
                    donation.AccountNumber = "";
                    donation.CardNumber = "";
                    donation.ExpiryDate = "";
                    donation.CVV = "";
                    donation.CardHolderName = "";
                    break;
                case "Nagad":
                    donation.BkashNumber = "";
                    donation.BankName = "";
                    donation.AccountNumber = "";
                    donation.CardNumber = "";
                    donation.ExpiryDate = "";
                    donation.CVV = "";
                    donation.CardHolderName = "";
                    break;
                case "BankTransfer":
                    donation.BkashNumber = "";
                    donation.NagadNumber = "";
                    donation.CardNumber = "";
                    donation.ExpiryDate = "";
                    donation.CVV = "";
                    donation.CardHolderName = "";
                    break;
                case "CreditCard":
                case "DebitCard":
                    donation.BkashNumber = "";
                    donation.NagadNumber = "";
                    donation.BankName = "";
                    donation.AccountNumber = "";
                    break;
            }

            donation.TransactionId = $"TXN-{Guid.NewGuid().ToString("N").Substring(0, 12).ToUpper()}";

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

        public async Task<Donation> GetDonationByTransactionIdAsync(string transactionId)
        {
            return await _context.Donations
                .FirstOrDefaultAsync(d => d.TransactionId == transactionId);
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

            var stats = new DonationStats
            {
                // Lifetime totals
                TotalDonations = donations.Sum(d => d.Amount),
                TotalDonors = donations.Select(d => d.Email).Distinct().Count(),

                // Remove monthly, just keep recent + method breakdown + all donations
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

                DonationsByMethod = donations
                    .GroupBy(d => d.PaymentMethod.ToString())
                    .ToDictionary(g => g.Key, g => g.Sum(d => d.Amount)),

                AllDonations = donations.OrderByDescending(d => d.CreatedAt).ToList()
            };

            return stats;
        }

    }
}
