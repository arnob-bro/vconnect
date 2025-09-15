using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VConnect.Database;
using VConnect.Models.Cases;
using VConnect.Services;

namespace VConnect.Controllers
{
    public class CaseController : Controller
    {
        private readonly IStudyService _service;
        private readonly ApplicationDbContext _db;

        public CaseController(IStudyService service, ApplicationDbContext db)
        {
            _service = service;
            _db = db;
        }

        // GET: /Case
        public async Task<IActionResult> Index()
        {
            var studies = (await _service.GetAllAsync()).ToList();

            var vm = new CaseIndexVM
            {
                FeaturedCase = studies.FirstOrDefault(),
                AllCases = studies,
                Categories = await _db.Categories
                                        .OrderBy(c => c.Name)
                                        .ToListAsync(),
                // If your DbSet is named Impact_stats instead of ImpactStats, swap the next line accordingly.
                ImpactStats = await _db.ImpactStats.FirstOrDefaultAsync() ?? new Impact_Stats()
            };

            return View(vm);
        }

        // GET: /Case/Details/5
        public async Task<IActionResult> Details(int id)
        {
            // Pull full study with images + milestones
            var study = await _db.Studies
                .Include(s => s.GalleryImages)
                .Include(s => s.Milestones)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (study == null) return NotFound();
            return View(study);
        }
    }
}
