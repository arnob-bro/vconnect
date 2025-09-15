
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using VConnect.Services;

namespace VConnect.Controllers
{
    public class FeedController : Controller
    {
        private readonly ISosService _sos;

        public FeedController(ISosService sos)
        {
            _sos = sos;
        }

        // GET: /Feed
        public async Task<IActionResult> Index()
        {
            // Grab latest posts for the feed
            var posts = await _sos.GetFeedAsync(skip: 0, take: 50);

            // Optional: last-visit highlight support
            ViewBag.LastVisitUtc = DateTime.UtcNow.AddHours(-12).ToString("o");

            return View(posts);
        }
    }
}
