using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using VConnect.Database;
using VConnect.Models;

namespace VConnect.Services
{
    public class ProfileDetailsService : IProfileDetailsService
    {
        private readonly ApplicationDbContext _db;

        public ProfileDetailsService(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<ProfileDetails?> GetByUserIdAsync(int userId)
        {
            return await _db.ProfileDetails
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.UserId == userId);
        }

        public async Task<ProfileDetails> EnsureForUserAsync(int userId)
        {
            var existing = await _db.ProfileDetails.FirstOrDefaultAsync(p => p.UserId == userId);
            if (existing != null) return existing;

            var user = await _db.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == userId);

            var profile = new ProfileDetails
            {
                UserId = userId,
                FirstName = user?.FirstName?.Trim() ?? string.Empty,
                LastName = user?.LastName?.Trim() ?? string.Empty,
                Email = user?.Email?.Trim() ?? string.Empty,
                CreatedAt = DateTimeOffset.UtcNow,
                UpdatedAt = DateTimeOffset.UtcNow
            };

            _db.ProfileDetails.Add(profile);
            await _db.SaveChangesAsync();
            return profile;
        }

        public async Task UpdateAsync(ProfileDetails incoming, IFormFile? upload, string webRootPath)
        {
            // Load or create entity for this user
            var entity = await _db.ProfileDetails
                .FirstOrDefaultAsync(p => p.ProfileDetailsId == incoming.ProfileDetailsId && p.UserId == incoming.UserId);

            if (entity == null)
            {
                entity = await _db.ProfileDetails.FirstOrDefaultAsync(p => p.UserId == incoming.UserId);
                if (entity == null)
                {
                    entity = new ProfileDetails
                    {
                        UserId = incoming.UserId,
                        CreatedAt = DateTimeOffset.UtcNow
                    };
                    _db.ProfileDetails.Add(entity);
                }
            }

            // Normalize text fields
            entity.FirstName = (incoming.FirstName ?? string.Empty).Trim();
            entity.LastName = (incoming.LastName ?? string.Empty).Trim();
            entity.Email = (incoming.Email ?? string.Empty).Trim();
            entity.PhoneNumber = incoming.PhoneNumber?.Trim();
            entity.City = incoming.City?.Trim();
            entity.Address = incoming.Address?.Trim();

            // DateOnly? just assign directly; column is "date"
            entity.DateOfBirth = incoming.DateOfBirth;

            // Timestamps
            entity.UpdatedAt = DateTimeOffset.UtcNow;

            // Optional file upload
            if (upload != null && upload.Length > 0)
            {
                var allowed = new[] { ".png", ".jpg", ".jpeg", ".gif", ".webp" };
                var ext = Path.GetExtension(upload.FileName).ToLowerInvariant();

                if (allowed.Contains(ext))
                {
                    var uploadsDir = Path.Combine(webRootPath, "uploads", "avatars");
                    Directory.CreateDirectory(uploadsDir);

                    var fileName = $"{Guid.NewGuid()}{ext}";
                    var fullPath = Path.Combine(uploadsDir, fileName);

                    using (var stream = new FileStream(fullPath, FileMode.Create))
                        await upload.CopyToAsync(stream);

                    entity.ProfilePictureUrl = $"/uploads/avatars/{fileName}";
                }
                // else: silently ignore or add ModelState validation upstream
            }

            await _db.SaveChangesAsync();
        }

        public async Task DeleteAsync(int userId)
        {
            var entity = await _db.ProfileDetails.FirstOrDefaultAsync(p => p.UserId == userId);
            if (entity != null)
            {
                _db.ProfileDetails.Remove(entity);
                await _db.SaveChangesAsync();
            }
        }
    }
}
