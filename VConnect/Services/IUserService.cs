using VConnect.Models;

namespace VConnect.Services
{
    public interface IUserService
    {
        Task<ApplicationUser> RegisterUserAsync(RegisterViewModel model);
        Task<ApplicationUser> AuthenticateUserAsync(LoginViewModel model);
        Task<bool> UserExistsAsync(string email);
    }
}
