using VConnect.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace VConnect.Services
{
    public interface IDonationService
    {
        Task<Donation> CreateDonationAsync(Donation donation);
        Task<List<Donation>> GetAllDonationsAsync();
        Task<Donation> GetDonationByIdAsync(int id);
        Task<bool> UpdateDonationStatusAsync(int id, DonationStatus status);
        Task<List<Donation>> GetRecentDonationsAsync(int count);
        Task<DonationStats> GetDonationStatsAsync();
    }
}
