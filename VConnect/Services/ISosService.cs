using System.Collections.Generic;
using System.Threading.Tasks;
using VConnect.Models.SOS;

namespace VConnect.Services
{
    public interface ISosService
    {
        Task<SosPost> CreatePostAsync(CreateSosPostDto dto, string ownerUserId, bool isGuest);

        Task<List<SosPost>> GetFeedAsync(int skip = 0, int take = 50, bool includeDeleted = false);

        Task<SosPost> GetPostAsync(int id);

        Task<SosComment> AddCommentAsync(
            int postId,
            string message,
            string userId,
            string authorName,
            int? parentCommentId = null
        );

        Task<bool> ToggleAvailabilityAsync(int id, string callerUserId, bool isAdmin = false);

        Task<bool> MarkCompletedAsync(int id, string callerUserId, bool isAdmin = false);

        Task<bool> DeletePostAsync(int id, string callerUserId, bool isAdmin = false);

        Task<bool> DeleteCommentAsync(int commentId, string callerUserId, bool isAdmin = false);
    }
}
