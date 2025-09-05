using Microsoft.AspNetCore.Mvc;
using VConnect.Models;
using System.Collections.Generic;
using System.Linq;

namespace VConnect.Controllers
{
    public class HeroesController : Controller
    {
        // GET: /Heroes or /MeetOurHeroes
        public IActionResult Index()
        {
            // In a real application, this would come from a database
            var model = new HeroesViewModel
            {
                FeaturedHeroes = GetFeaturedHeroes(),
                AllHeroes = GetAllHeroes(),
                TopContributors = GetTopContributors(),
                TotalHeroes = 1250,
                TotalHours = 25600,
                TotalProjects = 450
            };

            return View("~/Views/Heroes/Index.cshtml", model);
        }

        // GET: /Heroes/Details/{id}
        public IActionResult Details(int id)
        {
            var hero = GetHeroById(id);
            if (hero == null)
            {
                return NotFound();
            }

            return View("~/Views/Heroes/Details.cshtml", hero);
        }

        private List<Hero> GetFeaturedHeroes()
        {
            return new List<Hero>
            {
                new Hero {
                    Id = 1,
                    FullName = "Abdur Rahman",
                    ProfileImage = "/images/heroes/1.jpg",
                    Role = "Environmental Volunteer",
                    Location = "Dhaka",
                    JoinedDate = new DateTime(2020, 3, 15),
                    HoursVolunteered = 320,
                    ProjectsCompleted = 12,
                    ShortBio = "Dedicated to creating greener communities through tree plantation drives.",
                    Skills = "Tree Planting, Community Outreach, Leadership",
                    IsFeatured = true
                },
                new Hero {
                    Id = 2,
                    FullName = "Fatima Ahmed",
                    ProfileImage = "/images/heroes/2.jpg",
                    Role = "Education Volunteer",
                    Location = "Chittagong",
                    JoinedDate = new DateTime(2019, 8, 22),
                    HoursVolunteered = 450,
                    ProjectsCompleted = 18,
                    ShortBio = "Passionate about educating underprivileged children in rural areas.",
                    Skills = "Teaching, Curriculum Development, Mentoring",
                    IsFeatured = true
                },
                new Hero {
                    Id = 3,
                    FullName = "Mohammed Rahim",
                    ProfileImage = "/images/heroes/3.jpg",
                    Role = "Disaster Relief Volunteer",
                    Location = "Cox's Bazar",
                    JoinedDate = new DateTime(2021, 1, 10),
                    HoursVolunteered = 280,
                    ProjectsCompleted = 8,
                    ShortBio = "Always first to respond during natural disasters and emergency situations.",
                    Skills = "First Aid, Crisis Management, Logistics",
                    IsFeatured = true
                }
            };
        }

        private List<Hero> GetAllHeroes()
        {
            var heroes = new List<Hero>
            {
                new Hero {
                    Id = 4,
                    FullName = "Sadia Islam",
                    ProfileImage = "/images/heroes/4.jpg",
                    Role = "Health Volunteer",
                    Location = "Rajshahi",
                    JoinedDate = new DateTime(2022, 5, 18),
                    HoursVolunteered = 150,
                    ProjectsCompleted = 6,
                    ShortBio = "Providing healthcare services to remote communities.",
                    Skills = "First Aid, Health Education, Counseling"
                },
                new Hero {
                    Id = 5,
                    FullName = "Tahmid Hassan",
                    ProfileImage = "/images/heroes/5.jpg",
                    Role = "Digital Literacy Volunteer",
                    Location = "Sylhet",
                    JoinedDate = new DateTime(2021, 9, 3),
                    HoursVolunteered = 220,
                    ProjectsCompleted = 9,
                    ShortBio = "Teaching digital skills to youth in underserved communities.",
                    Skills = "Computer Training, Digital Literacy, Workshop Facilitation"
                },
                new Hero {
                    Id = 6,
                    FullName = "Nusrat Jahan",
                    ProfileImage = "/images/heroes/6.jpg",
                    Role = "Women Empowerment Volunteer",
                    Location = "Khulna",
                    JoinedDate = new DateTime(2020, 11, 30),
                    HoursVolunteered = 380,
                    ProjectsCompleted = 14,
                    ShortBio = "Empowering women through skill development and education.",
                    Skills = "Vocational Training, Women Leadership, Entrepreneurship"
                }
            };

            heroes.AddRange(GetFeaturedHeroes());
            return heroes;
        }

        private List<Hero> GetTopContributors()
        {
            return GetAllHeroes()
                .OrderByDescending(h => h.HoursVolunteered)
                .Take(6)
                .ToList();
        }

        private Hero GetHeroById(int id)
        {
            return GetAllHeroes().FirstOrDefault(h => h.Id == id);
        }
    }
}