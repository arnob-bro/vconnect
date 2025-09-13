using VConnect.Models.Enums;
namespace VConnect.Models.Events
{
    public class EventApplication
    {
        public int EventApplicationId { get; set; }  // PK

        public int EventId { get; set; }             // FK → Event
        public int EventRoleId { get; set; }         // FK → EventRole
        public int UserId { get; set; }              // FK → ApplicationUser

        public ApplicationStatus Status { get; set; } = ApplicationStatus.Pending;
        public DateTime AppliedAt { get; set; } = DateTime.UtcNow;

        // Navigation
        public Event Event { get; set; } = null!;
        public Role Role { get; set; } = null!;
        public ApplicationUser User { get; set; } = null!;
    }
}
