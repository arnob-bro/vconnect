using System.Collections.Generic;

namespace VConnect.Models.Cases
{
    public class CaseIndexVM
    {
        public Study FeaturedCase { get; set; }
        public List<Study> AllCases { get; set; } = new();
        public List<Category> Categories { get; set; } = new();
        public Impact_Stats ImpactStats { get; set; } = new();
    }
}
