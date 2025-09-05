using System.Collections.Generic;

namespace VConnect.Models
{
    public class CaseStudyPage
    {
        public CaseStudy FeaturedCase { get; set; }
        public List<CaseStudy> AllCases { get; set; }
        public List<CaseCategory> Categories { get; set; }
        public ImpactStats ImpactStats { get; set; }
    }

    public class CaseStudy
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Subtitle { get; set; }
        public string ImageUrl { get; set; }
        public string Category { get; set; }
        public string Location { get; set; }
        public string Duration { get; set; }
        public int Volunteers { get; set; }
        public int Beneficiaries { get; set; }
        public string ShortDescription { get; set; }
        public string Challenge { get; set; }
        public string Solution { get; set; }
        public string Results { get; set; }
        public string Status { get; set; }
        public string Budget { get; set; }
        public string Partners { get; set; }
        public List<string> GalleryImages { get; set; } = new List<string>();
        public List<CaseMilestone> Milestones { get; set; } = new List<CaseMilestone>();
    }

    public class CaseCategory
    {
        public string Name { get; set; }
        public string Icon { get; set; }
        public int Count { get; set; }
        public bool IsActive { get; set; }
    }

    public class ImpactStats
    {
        public int TotalCases { get; set; }
        public int CommunitiesImpacted { get; set; }
        public int VolunteersInvolved { get; set; }
        public int SuccessRate { get; set; }
    }

    public class CaseMilestone
    {
        public string Date { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
    }
}