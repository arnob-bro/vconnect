using System.Threading.Tasks;
using VConnect.Models;
namespace VConnect.Services
{
    public interface IProfileDetailsService
    {
        Task<ProfileDetails?> GetByUserIdAsync(int userId);
        Task UpdateAsync(ProfileDetails profile);
        Task DeleteAsync(int userId);
    }
}
