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

        public async Task<ApplicationUser> RegisterUserAsync(RegisterViewModel model)
        {
            if (await UserExistsAsync(model.Email))
                return null;

            var user = new ApplicationUser
            {
                Email = model.Email,
                Password = model.Password, // ⚠️ In production, hash passwords!
                FirstName = model.FirstName,
                LastName = model.LastName
            };

            _dbContext.Users.Add(user);
            await _dbContext.SaveChangesAsync();
            return user;
        }

        public async Task<ApplicationUser> AuthenticateUserAsync(LoginViewModel model)
        {
            return await _dbContext.Users
                .FirstOrDefaultAsync(u => u.Email == model.Email && u.Password == model.Password);
        }

        public async Task<bool> UserExistsAsync(string email)
        {
            return await _dbContext.Users.AnyAsync(u => u.Email == email);
        }
    }
}
