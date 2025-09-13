using Microsoft.EntityFrameworkCore;
using VConnect.Database;
using VConnect.Models;

namespace VConnect.Services
{
    public class UserService : IUserService
    {
        private readonly ApplicationDbContext _dbContext;

        public UserService(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<ApplicationUser?> RegisterUserAsync(RegisterViewModel model)
        {
            var email = model.Email?.Trim().ToLowerInvariant() ?? string.Empty;

            if (await UserExistsAsync(email))
                return null;

            // Hash the password
            var hashed = BCrypt.Net.BCrypt.HashPassword(model.Password);

            var user = new ApplicationUser
            {
                Email = email,
                Password = hashed,
                FirstName = model.FirstName?.Trim() ?? string.Empty,
                LastName = model.LastName?.Trim() ?? string.Empty,
                // default everyone to Volunteer unless it’s the special admin
                Role = email == "admin@gmail.com" ? "Admin" : "Volunteer"
            };

            _dbContext.Users.Add(user);
            await _dbContext.SaveChangesAsync();
            return user;
        }

        public async Task<ApplicationUser?> AuthenticateUserAsync(LoginViewModel model)
        {
            var email = model.Email?.Trim().ToLowerInvariant() ?? string.Empty;

            var user = await _dbContext.Users
                .FirstOrDefaultAsync(u => u.Email == email);

            if (user == null)
                return null;

            // Verify hashed password
            var ok = BCrypt.Net.BCrypt.Verify(model.Password, user.Password);
            return ok ? user : null;
        }

        public async Task<bool> UserExistsAsync(string email)
        {
            var norm = email?.Trim().ToLowerInvariant() ?? string.Empty;
            return await _dbContext.Users.AnyAsync(u => u.Email == norm);
        }
    }
}
