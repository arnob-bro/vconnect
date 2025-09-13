namespace VConnect.Models.Events
{
    public class Participation
    {
        public int EventParticipationId { get; set; }  // PK

        public int EventId { get; set; }
        public int UserId { get; set; }
        public int EventRoleId { get; set; }

        public int HoursContributed { get; set; } = 0;
        public DateTime CompletedAt { get; set; } = DateTime.UtcNow;

        // Navigation
        public Event Event { get; set; } = null!;
        public Role Role { get; set; } = null!;
        public ApplicationUser User { get; set; } = null!;

    }
}
