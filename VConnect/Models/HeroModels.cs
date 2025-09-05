using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace VConnect.Models
{
    public class Hero
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Full Name")]
        public string FullName { get; set; }

        [Display(Name = "Profile Image")]
        public string ProfileImage { get; set; }

        [Required]
        public string Role { get; set; }

        [Required]
        public string Location { get; set; }

        [Display(Name = "Joined Date")]
        [DataType(DataType.Date)]
        public DateTime JoinedDate { get; set; }

        [Display(Name = "Hours Volunteered")]
        public int HoursVolunteered { get; set; }

        [Display(Name = "Projects Completed")]
        public int ProjectsCompleted { get; set; }

        [Display(Name = "Short Bio")]
        public string ShortBio { get; set; }

        public string Skills { get; set; }

        [Display(Name = "Social Media Links")]
        public string SocialMediaLinks { get; set; }

        [Display(Name = "Featured Hero")]
        public bool IsFeatured { get; set; }

        [Display(Name = "Active Status")]
        public bool IsActive { get; set; } = true;
    }

    public class HeroesViewModel
    {
        public List<Hero> FeaturedHeroes { get; set; } = new List<Hero>();
        public List<Hero> AllHeroes { get; set; } = new List<Hero>();
        public List<Hero> TopContributors { get; set; } = new List<Hero>();
        public int TotalHeroes { get; set; }
        public int TotalHours { get; set; }
        public int TotalProjects { get; set; }
    }

    public class HeroCategory
    {
        public string Name { get; set; }
        public string Icon { get; set; }
        public int Count { get; set; }
    }
}