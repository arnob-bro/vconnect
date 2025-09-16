using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using VConnect.Services;
using VConnect.Filters;

namespace VConnect.Areas.Admin.Controllers
{
    [Area("Admin")]
    [AdminOnly]
    [Route("Admin/[controller]/[action]")]
    [Route("Admin/Emergencies/[action]")] 
    public class PostsController : Controller
    {
        private readonly ISosService _sos;

        public PostsController(ISosService sos)
        {
            _sos = sos;
        }

        // GET: /Admin/Emergencies or /Admin/Posts
        [HttpGet, Route("/Admin/Emergencies"), Route("/Admin/Posts")]
        public async Task<IActionResult> Index(int page = 1, int pageSize = 50)
        {
            if (page < 1) page = 1;
            if (pageSize <= 0 || pageSize > 200) pageSize = 50;

            var skip = (page - 1) * pageSize;
            var posts = await _sos.GetFeedAsync(skip: skip, take: pageSize, includeDeleted: true);

            return View(posts); 
        }

        // GET: /Admin/Emergencies/Details/5
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var post = await _sos.GetPostAsync(id);
            if (post == null) return NotFound();

            return View(post); 
        }

        // POST: /Admin/Emergencies/DeletePost/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeletePost(int id)
        {
            var callerUserId = User?.Identity?.Name ?? "";
            var ok = await _sos.DeletePostAsync(id, callerUserId, isAdmin: true);
            if (!ok) return Forbid();

            return RedirectToAction(nameof(Index));
        }

        // POST: /Admin/Emergencies/DeleteComment
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteComment(int commentId, int postId)
        {
            var callerUserId = User?.Identity?.Name ?? "";
            var ok = await _sos.DeleteCommentAsync(commentId, callerUserId, isAdmin: true);
            if (!ok) return Forbid();

            return RedirectToAction(nameof(Details), new { id = postId });
        }
    }
}
