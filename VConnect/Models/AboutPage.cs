using System.Collections.Generic;

namespace VConnect.Models
{
    public class AboutPage
    {
        public string MissionStatement { get; set; }
        public string History { get; set; }
        public AboutStats Stats { get; set; }
        public List<TeamMember> TeamMembers { get; set; }
        public List<ValueItem> Values { get; set; }
        public List<TimelineEvent> TimelineEvents { get; set; }
    }

    public class AboutStats
    {
        public int Volunteers { get; set; }
        public int ProjectsCompleted { get; set; }
        public int CommunitiesServed { get; set; }
        public int HoursVolunteered { get; set; }
    }

    public class TeamMember
    {
        public string Name { get; set; }
        public string Position { get; set; }
        public string ImageUrl { get; set; }
        public string Bio { get; set; }
        public string SocialLinks { get; set; }
    }

    public class ValueItem
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string Icon { get; set; }
    }

    public class TimelineEvent
    {
        public int Year { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
    }
}