using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VConnect.Database;
using VConnect.Services;
using VConnect.Models;
using VConnect.Models.Cases;

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

        public async Task<IActionResult> Index()
        {
            var studies = (await _service.GetAllAsync()).ToList();

            var vm = new CaseStudyPage
            {
                FeaturedCase = MapToFrontend(studies.FirstOrDefault()),
                AllCases = studies.Select(MapToFrontend).ToList(),
                Categories = await _db.Categories
                                      .Select(c => new CaseCategory
                                      {
                                          Name = c.Name,
                                          Icon = c.Icon,
                                          Count = c.Count,
                                          IsActive = c.IsActive
                                      })
                                      .ToListAsync(),
                ImpactStats = await _db.ImpactStats
                    .Select(i => new ImpactStats
                    {
                        TotalCases = i.TotalCases,
                        CommunitiesImpacted = i.CommunitiesImpacted,
                        VolunteersInvolved = i.VolunteersInvolved,
                        SuccessRate = i.SuccessRate
                    })
                    .FirstOrDefaultAsync() ?? new ImpactStats()
            };

            return View(vm);
        }

        public async Task<IActionResult> Details(int id)
        {
            var study = await _service.GetByIdAsync(id);
            if (study == null) return NotFound();
            return View(MapToFrontend(study));
        }

        private CaseStudy MapToFrontend(Study s)
        {
            if (s == null) return null;

            return new CaseStudy
            {
                Id = s.Id,
                Title = s.Title,
                Subtitle = s.Subtitle,
                ImageUrl = s.ImageUrl,
                Category = s.Category,
                Location = s.Location,
                Duration = s.Duration,
                Volunteers = s.Volunteers,
                Beneficiaries = s.Beneficiaries,
                ShortDescription = s.ShortDescription,
                Challenge = s.Challenge,
                Solution = s.Solution,
                Results = s.Results,
                Status = s.Status,
                Budget = s.Budget,
                Partners = s.Partners,
                GalleryImages = s.GalleryImages?.Select(g => g.ImageUrl).ToList() ?? new(),
                Milestones = s.Milestones?.Select(m => new CaseMilestone
                {
                    Date = m.Date.ToShortDateString(),
                    Title = m.Title,
                    Description = m.Description
                }).ToList() ?? new()
            };
        }
    }
}
