using Microsoft.EntityFrameworkCore;
using VConnect.Database;
using VConnect.Models;
namespace VConnect.Services
{
    public class ProfileDetailsService : IProfileDetailsService
    {
        private readonly ApplicationDbContext _context;

        public ProfileDetailsService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ProfileDetails?> GetByUserIdAsync(int userId)
        {
            return await _context.ProfileDetails
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.UserId == userId);
        }

        public async Task UpdateAsync(ProfileDetails profile)
        {
            _context.ProfileDetails.Update(profile);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int userId)
        {
            var profile = await _context.ProfileDetails
                .FirstOrDefaultAsync(p => p.UserId == userId);

            if (profile != null)
            {
                _context.ProfileDetails.Remove(profile);
                await _context.SaveChangesAsync();
            }
        }
    }
}
