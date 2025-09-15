using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using VConnect.Database;          // Your DbContext namespace
using VConnect.Models.SOS;        // SosPost, SosComment, CreateSosPostDto

namespace VConnect.Services
{
    public class SosService : ISosService
    {
        private readonly ApplicationDbContext _db;

        public SosService(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<SosPost> CreatePostAsync(CreateSosPostDto dto, string ownerUserId, bool isGuest)
        {
            if (dto == null) throw new ArgumentNullException(nameof(dto));

            var post = new SosPost
            {
                OwnerUserId = string.IsNullOrWhiteSpace(ownerUserId) ? null : ownerUserId,
                IsGuest = isGuest,
                Name = dto.Name?.Trim(),
                Contact = dto.Contact?.Trim(),
                Location = dto.Location?.Trim(),
                Latitude = dto.Latitude,
                Longitude = dto.Longitude,
                EmergencyType = dto.EmergencyType?.Trim(),
                Description = dto.Description?.Trim(),
                Status = "Urgent",
                IsAcceptingHelp = true,
                IsVisible = true,
                IsDeleted = false,
                CreatedAt = DateTime.UtcNow
            };

            _db.SosPosts.Add(post);
            await _db.SaveChangesAsync();
            return post;
        }

        // EF Core 7 filtered Include + ThenInclude for feed
        public async Task<List<SosPost>> GetFeedAsync(int skip = 0, int take = 50, bool includeDeleted = false)
        {
            if (skip < 0) skip = 0;
            if (take <= 0 || take > 200) take = 50;

            return await _db.SosPosts
                .Where(p => p.IsVisible && (includeDeleted || !p.IsDeleted))
                .OrderByDescending(p => p.CreatedAt)
                // bring all not-deleted comments and one level of not-deleted replies
                .Include(p => p.Comments
                    .Where(c => !c.IsDeleted)
                    .OrderBy(c => c.CreatedAt))
                    .ThenInclude(c => c.Replies
                        .Where(r => !r.IsDeleted)
                        .OrderBy(r => r.CreatedAt))
                .AsSplitQuery()
                .AsNoTracking()
                .Skip(skip)
                .Take(take)
                .ToListAsync();
        }

        public async Task<SosPost> GetPostAsync(int id)
        {
            return await _db.SosPosts
                .Where(p => p.Id == id && p.IsVisible && !p.IsDeleted)
                .Include(p => p.Comments
                    .Where(c => !c.IsDeleted)
                    .OrderBy(c => c.CreatedAt))
                    .ThenInclude(c => c.Replies
                        .Where(r => !r.IsDeleted)
                        .OrderBy(r => r.CreatedAt))
                .AsSplitQuery()
                .AsNoTracking()
                .FirstOrDefaultAsync();
        }

        public async Task<SosComment> AddCommentAsync(
            int postId,
            string message,
            string userId,
            string authorName,
            int? parentCommentId = null)
        {
            // Basic guard rails
            var postExists = await _db.SosPosts.AnyAsync(p => p.Id == postId && p.IsVisible && !p.IsDeleted);
            if (!postExists) return null;

            if (parentCommentId.HasValue)
            {
                var parentOk = await _db.SosComments.AnyAsync(c =>
                    c.Id == parentCommentId.Value &&
                    !c.IsDeleted &&
                    c.SosPostId == postId);
                if (!parentOk) return null;
            }

            var trimmedMessage = (message ?? string.Empty).Trim();
            if (trimmedMessage.Length == 0) return null;
            if (trimmedMessage.Length > 1000) trimmedMessage = trimmedMessage.Substring(0, 1000);

            var comment = new SosComment
            {
                SosPostId = postId,
                ParentCommentId = parentCommentId,
                // nullable if guests allowed; make sure DB column is nullable
                UserId = string.IsNullOrWhiteSpace(userId) ? null : userId,
                AuthorName = string.IsNullOrWhiteSpace(authorName) ? "Guest" : authorName.Trim(),
                Message = trimmedMessage,
                CreatedAt = DateTime.UtcNow,
                IsDeleted = false
            };

            _db.SosComments.Add(comment);
            await _db.SaveChangesAsync();
            return comment;
        }

        public async Task<bool> ToggleAvailabilityAsync(int id, string callerUserId, bool isAdmin = false)
        {
            var post = await _db.SosPosts.FirstOrDefaultAsync(p => p.Id == id);
            if (post == null || post.IsDeleted) return false;

            if (!CanManagePost(post, callerUserId, isAdmin)) return false;

            post.IsAcceptingHelp = !post.IsAcceptingHelp;
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<bool> MarkCompletedAsync(int id, string callerUserId, bool isAdmin = false)
        {
            var post = await _db.SosPosts.FirstOrDefaultAsync(p => p.Id == id);
            if (post == null || post.IsDeleted) return false;

            if (!CanManagePost(post, callerUserId, isAdmin)) return false;

            post.Status = "Completed";
            post.IsAcceptingHelp = false;
            post.CompletedAt = DateTime.UtcNow;
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeletePostAsync(int id, string callerUserId, bool isAdmin = false)
        {
            var post = await _db.SosPosts
                .Include(p => p.Comments)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (post == null || post.IsDeleted) return false;
            if (!CanManagePost(post, callerUserId, isAdmin)) return false;

            post.IsDeleted = true;
            post.IsVisible = false;

            // Soft-delete existing comments so they don't linger
            foreach (var c in post.Comments)
                c.IsDeleted = true;

            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteCommentAsync(int commentId, string callerUserId, bool isAdmin = false)
        {
            var comment = await _db.SosComments
                .Include(c => c.Replies)
                .Include(c => c.SosPost)
                .FirstOrDefaultAsync(c => c.Id == commentId);

            if (comment == null || comment.IsDeleted) return false;

            // Allow: comment author OR post owner OR admin
            var canDelete = isAdmin
                            || (!string.IsNullOrWhiteSpace(comment.UserId) && comment.UserId == callerUserId)
                            || (!string.IsNullOrWhiteSpace(comment.SosPost?.OwnerUserId) && comment.SosPost.OwnerUserId == callerUserId);

            if (!canDelete) return false;

            comment.IsDeleted = true;
            foreach (var r in comment.Replies)
                r.IsDeleted = true;

            await _db.SaveChangesAsync();
            return true;
        }

        private static bool CanManagePost(SosPost post, string callerUserId, bool isAdmin)
        {
            if (isAdmin) return true;
            if (string.IsNullOrWhiteSpace(post.OwnerUserId)) return false; // guest post has no verifiable owner
            return string.Equals(post.OwnerUserId, callerUserId, StringComparison.Ordinal);
        }
    }
}
