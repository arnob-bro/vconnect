namespace VConnect.Models
{
    public class ActivityCard
    {
        public string Title { get; set; }
        public string City { get; set; }
        public string DateText { get; set; }   // e.g., "Aug 17th, 2025"
        public string ImageUrl { get; set; }   // ~/images/activities/1.jpg
        public string LinkUrl { get; set; }    // details page (optional)
    }

    public class ActivitiesSectionViewModel
    {
        public string Eyebrow { get; set; } = "Activities";
        public string HeadingLine1 { get; set; } = "VConnect's";
        public string HeadingLine2 { get; set; } = "Regional";
        public string HeadingLine3 { get; set; } = "Activities";
        public string ViewAllUrl { get; set; } = "#";
        public List<ActivityCard> Cards { get; set; } = new();
        public int CardsPerSlide { get; set; } = 3; // desktop
        public int IntervalMs { get; set; } = 4500; // auto-rotate
    }
}
