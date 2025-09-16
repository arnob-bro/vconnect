using VConnect.Models.Enums;

namespace VConnect.Models.Events
{
    public class Event
    {
        public int EventId { get; set; }   // Primary Key

        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;

        public string Location { get; set; } = string.Empty;
        public DateTime StartDateTime { get; set; }
        public DateTime EndDateTime { get; set; }

        // Capacity (number of volunteers required)
        public int Capacity { get; set; }

        // Payment / Compensation (can be money or "Free")
        public string Compensation { get; set; } = "Free";

        // Event Status (Active, Completed, Cancelled)
        public EventStatus Status { get; set; } = EventStatus.Active;

        // Relationships
        public ICollection<EventApplication>? Applications { get; set; }
        public ICollection<Participation>? Participations { get; set; }

        // Created by
        public int CreatedById { get; set; }                // FK → ApplicationUser

        // Audit
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
