using Microsoft.AspNetCore.Mvc;
using VConnect.Models;
using System.Collections.Generic;

namespace VConnect.Controllers
{
    public class AboutController : Controller
    {
        // GET: /About
        public IActionResult Index()
        {
            var model = new AboutPage
            {
                MissionStatement = "VConnect is the dynamic youth wing of JAAGO Foundation, providing a powerful platform driving youth-led action to accelerate progress toward achieving the Sustainable Development Goals (SDGs).",
                History = "Founded in 2011, VConnect has grown from a small student initiative to a nationwide movement with over 50,000 volunteers across Bangladesh.",
                Stats = new AboutStats
                {
                    Volunteers = 50000,
                    ProjectsCompleted = 1200,
                    CommunitiesServed = 500,
                    HoursVolunteered = 2500000
                },
                TeamMembers = GetTeamMembers(),
                Values = GetValues(),
                TimelineEvents = GetTimelineEvents()
            };

            return View("~/Views/About/Index.cshtml", model);
        }

        private List<TeamMember> GetTeamMembers()
        {
            return new List<TeamMember>
            {
                new TeamMember
                {
                    Name = "Korvi Rakshand",
                    Position = "Founder & Executive Director",
                    ImageUrl = "/images/team/korvi.jpg",
                    Bio = "Visionary leader and social entrepreneur dedicated to youth empowerment.",
                    SocialLinks = "twitter|https://twitter.com/korvirakshand,linkedin|https://linkedin.com/in/korvirakshand"
                },
                new TeamMember
                {
                    Name = "Dr. Saleh Ahmed",
                    Position = "Program Director",
                    ImageUrl = "/images/team/saleh.jpg",
                    Bio = "Experienced development professional with expertise in community engagement.",
                    SocialLinks = "linkedin|https://linkedin.com/in/salehahmed"
                },
                new TeamMember
                {
                    Name = "Fatima Begum",
                    Position = "Volunteer Coordinator",
                    ImageUrl = "/images/team/fatima.jpg",
                    Bio = "Passionate about connecting youth with meaningful opportunities.",
                    SocialLinks = "facebook|https://facebook.com/fatima.begum,instagram|https://instagram.com/fatima_begum"
                }
            };
        }

        private List<ValueItem> GetValues()
        {
            return new List<ValueItem>
            {
                new ValueItem
                {
                    Title = "Youth Empowerment",
                    Description = "We believe in the power of young people to drive positive change.",
                    Icon = "fas fa-users"
                },
                new ValueItem
                {
                    Title = "Community First",
                    Description = "Our work is always guided by the needs and aspirations of local communities.",
                    Icon = "fas fa-hands-helping"
                },
                new ValueItem
                {
                    Title = "Sustainable Impact",
                    Description = "We focus on creating lasting change through sustainable development approaches.",
                    Icon = "fas fa-seedling"
                },
                new ValueItem
                {
                    Title = "Innovation",
                    Description = "We embrace creative solutions to address complex social challenges.",
                    Icon = "fas fa-lightbulb"
                }
            };
        }

        private List<TimelineEvent> GetTimelineEvents()
        {
            return new List<TimelineEvent>
            {
                new TimelineEvent
                {
                    Year = 2011,
                    Title = "Foundation",
                    Description = "VConnect was established as the youth wing of JAAGO Foundation"
                },
                new TimelineEvent
                {
                    Year = 2013,
                    Title = "First National Campaign",
                    Description = "Launched our first nationwide volunteer campaign with 5,000 participants"
                },
                new TimelineEvent
                {
                    Year = 2015,
                    Title = "SDG Alignment",
                    Description = "Realigned our mission to support the UN Sustainable Development Goals"
                },
                new TimelineEvent
                {
                    Year = 2018,
                    Title = "Digital Platform",
                    Description = "Launched our online volunteer management system"
                },
                new TimelineEvent
                {
                    Year = 2020,
                    Title = "COVID Response",
                    Description = "Mobilized 10,000 volunteers for pandemic relief efforts"
                },
                new TimelineEvent
                {
                    Year = 2023,
                    Title = "50K Volunteers",
                    Description = "Reached milestone of 50,000 registered volunteers nationwide"
                }
            };
        }
    }
}