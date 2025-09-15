using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using VConnect.Services;

namespace VConnect.Controllers
{
    public class SosController : Controller
    {
        private readonly ISosService _sosService;

        public SosController(ISosService sosService)
        {
            _sosService = sosService;
        }

        // Optional landing: reuse the Feed view so /Sos still works
        public async Task<IActionResult> Index()
        {
            var posts = await _sosService.GetFeedAsync();
            ViewBag.LastVisitUtc = System.DateTime.UtcNow;
            return View("~/Views/Feed/Index.cshtml", posts);
        }

        // Keep Details if you plan to add /Views/Sos/Details.cshtml later.
        // Do NOT redirect here after mutations unless that view actually exists.
        public async Task<IActionResult> Details(int id)
        {
            var post = await _sosService.GetPostAsync(id);
            if (post == null) return NotFound();
            // Let MVC use Views/Sos/Details.cshtml by convention if you add it.
            return View(post);
        }

        // Create new SOS post
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateSosPostDto dto)
        {
            var userId = User?.Identity?.IsAuthenticated == true
                ? User.FindFirst("sub")?.Value ?? User.FindFirst("userid")?.Value
                : null;

            var isGuest = string.IsNullOrEmpty(userId);

            var post = await _sosService.CreatePostAsync(dto, userId, isGuest);
            if (post == null) return BadRequest("Failed to create post");

            // Back to feed root (could jump to new post if service returns its id)
            return RedirectToAction("Index", "Feed");
        }

        // Add comment (top-level or reply)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddComment(int postId, string message, int? parentCommentId = null)
        {
            var userId = User?.Identity?.IsAuthenticated == true
                ? User.FindFirst("sub")?.Value ?? User.FindFirst("userid")?.Value
                : null;

            var authorName = User?.Identity?.IsAuthenticated == true
                ? (User.Identity?.Name ?? "User")
                : "Guest";

            var comment = await _sosService.AddCommentAsync(postId, message, userId, authorName, parentCommentId);
            if (comment == null) return BadRequest("Failed to add comment");

            // Stay on feed and jump to the post card (ensure id="post-@Model.Id" in _PostPartial)
            return Redirect(Url.Action("Index", "Feed") + $"#post-{postId}");
        }

        // Toggle accepting help
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleAvailability(int id)
        {
            var userId = User.FindFirst("sub")?.Value ?? User.FindFirst("userid")?.Value;
            var success = await _sosService.ToggleAvailabilityAsync(id, userId);
            if (!success) return Forbid();

            return Redirect(Url.Action("Index", "Feed") + $"#post-{id}");
        }

        // Mark completed
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MarkCompleted(int id)
        {
            var userId = User.FindFirst("sub")?.Value ?? User.FindFirst("userid")?.Value;
            var success = await _sosService.MarkCompletedAsync(id, userId);
            if (!success) return Forbid();

            return Redirect(Url.Action("Index", "Feed") + $"#post-{id}");
        }

        // Delete post
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = User.FindFirst("sub")?.Value ?? User.FindFirst("userid")?.Value;
            var success = await _sosService.DeletePostAsync(id, userId);
            if (!success) return Forbid();

            // After delete there’s nothing to anchor to
            return RedirectToAction("Index", "Feed");
        }

        // Delete comment
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteComment(int commentId, int postId)
        {
            var userId = User.FindFirst("sub")?.Value ?? User.FindFirst("userid")?.Value;
            var success = await _sosService.DeleteCommentAsync(commentId, userId);
            if (!success) return Forbid();

            return Redirect(Url.Action("Index", "Feed") + $"#post-{postId}");
        }
    }
}
