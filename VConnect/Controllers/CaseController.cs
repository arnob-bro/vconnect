using Microsoft.AspNetCore.Mvc;
using VConnect.Models;
using System.Collections.Generic;

namespace VConnect.Controllers
{
    public class CaseController : Controller
    {
        // GET: /Case or /CaseStudy
        public IActionResult Index()
        {
            var model = new CaseStudyPage
            {
                FeaturedCase = GetFeaturedCase(),
                AllCases = GetAllCases(),
                Categories = GetCategories(),
                ImpactStats = new ImpactStats
                {
                    TotalCases = 45,
                    CommunitiesImpacted = 120,
                    VolunteersInvolved = 2500,
                    SuccessRate = 92
                }
            };

            return View("~/Views/Case/Index.cshtml", model);
        }

        // GET: /Case/Details/{id}
        public IActionResult Details(int id)
        {
            var caseStudy = GetCaseById(id);
            if (caseStudy == null)
            {
                return NotFound();
            }

            return View("~/Views/Case/Details.cshtml", caseStudy);
        }

        private CaseStudy GetFeaturedCase()
        {
            return new CaseStudy
            {
                Id = 1,
                Title = "Green Bangladesh Initiative",
                Subtitle = "Transforming Urban Landscapes Through Community-led Afforestation",
                ImageUrl = "/images/cases/featured.jpg",
                Category = "Environment",
                Location = "Dhaka, Chattogram, Sylhet",
                Duration = "2022-2023",
                Volunteers = 1200,
                Beneficiaries = 50000,
                ShortDescription = "A comprehensive urban afforestation project that transformed concrete landscapes into green spaces across three major cities.",
                Challenge = "Rapid urbanization in Bangladesh has led to significant loss of green spaces, affecting air quality, biodiversity, and community well-being.",
                Solution = "We mobilized youth volunteers to create urban gardens, vertical forests, and community parks through innovative planting techniques.",
                Results = "Planted over 50,000 trees, created 12 community parks, improved air quality by 15%, and engaged local communities in sustainable maintenance.",
                Status = "Completed",
                Budget = "৳25,00,000",
                Partners = "Ministry of Environment, City Corporations, Local Businesses"
            };
        }

        private List<CaseStudy> GetAllCases()
        {
            return new List<CaseStudy>
            {
                new CaseStudy
                {
                    Id = 2,
                    Title = "Digital Literacy Drive",
                    Subtitle = "Bridging the Digital Divide in Rural Communities",
                    ImageUrl = "/images/cases/digital.jpg",
                    Category = "Education",
                    Location = "Rajshahi Division",
                    Duration = "2023",
                    Volunteers = 450,
                    Beneficiaries = 15000,
                    ShortDescription = "Empowering rural youth with essential digital skills for the 21st century job market.",
                    Status = "Ongoing"
                },
                new CaseStudy
                {
                    Id = 3,
                    Title = "Clean Water Access",
                    Subtitle = "Sustainable Water Solutions for Coastal Communities",
                    ImageUrl = "/images/cases/water.jpg",
                    Category = "Health",
                    Location = "Khulna Division",
                    Duration = "2021-2022",
                    Volunteers = 300,
                    Beneficiaries = 25000,
                    ShortDescription = "Installing rainwater harvesting systems and water purification units in salinity-affected areas.",
                    Status = "Completed"
                },
                new CaseStudy
                {
                    Id = 4,
                    Title = "Women Entrepreneurship",
                    Subtitle = "Economic Empowerment Through Skill Development",
                    ImageUrl = "/images/cases/women.jpg",
                    Category = "Livelihood",
                    Location = "Countrywide",
                    Duration = "2022-2024",
                    Volunteers = 600,
                    Beneficiaries = 5000,
                    ShortDescription = "Creating sustainable livelihood opportunities for women through vocational training and micro-enterprise support.",
                    Status = "Ongoing"
                },
                new CaseStudy
                {
                    Id = 5,
                    Title = "Disaster Resilience",
                    Subtitle = "Building Community Capacity for Climate Adaptation",
                    ImageUrl = "/images/cases/disaster.jpg",
                    Category = "Environment",
                    Location = "Coastal Regions",
                    Duration = "2023",
                    Volunteers = 800,
                    Beneficiaries = 35000,
                    ShortDescription = "Preparing vulnerable communities for climate-induced disasters through early warning systems and training.",
                    Status = "Ongoing"
                },
                new CaseStudy
                {
                    Id = 6,
                    Title = "Youth Leadership",
                    Subtitle = "Developing Next-Generation Community Leaders",
                    ImageUrl = "/images/cases/youth.jpg",
                    Category = "Education",
                    Location = "Nationwide",
                    Duration = "2021-2023",
                    Volunteers = 2000,
                    Beneficiaries = 10000,
                    ShortDescription = "Leadership training and mentorship program for young change-makers across Bangladesh.",
                    Status = "Completed"
                }
            };
        }

        private List<CaseCategory> GetCategories()
        {
            return new List<CaseCategory>
            {
                new CaseCategory { Name = "All", Count = 6, IsActive = true },
                new CaseCategory { Name = "Environment", Count = 2, Icon = "fas fa-tree" },
                new CaseCategory { Name = "Education", Count = 2, Icon = "fas fa-graduation-cap" },
                new CaseCategory { Name = "Health", Count = 1, Icon = "fas fa-heartbeat" },
                new CaseCategory { Name = "Livelihood", Count = 1, Icon = "fas fa-hands-helping" }
            };
        }

        private CaseStudy GetCaseById(int id)
        {
            var allCases = new List<CaseStudy> { GetFeaturedCase() };
            allCases.AddRange(GetAllCases());
            return allCases.Find(c => c.Id == id);
        }
    }
}