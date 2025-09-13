using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using VConnect.Models;

namespace VConnect.Services
{
    public interface IProfileDetailsService
    {
        Task<ProfileDetails?> GetByUserIdAsync(int userId);
        Task<ProfileDetails> EnsureForUserAsync(int userId);
        Task UpdateAsync(ProfileDetails incoming, IFormFile? upload, string webRootPath);
        Task DeleteAsync(int userId);
    }
}
