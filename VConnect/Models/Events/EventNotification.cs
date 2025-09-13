using System;

namespace VConnect.Models.Events
{
    public class EventNotification
    {
        public int EventNotificationId { get; set; }  // PK
        public int UserId { get; set; }
        public int EventId { get; set; }

        public string Message { get; set; } = string.Empty;
        public bool IsRead { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation
        public Event Event { get; set; } = null!;
        public ApplicationUser User { get; set; } = null!;
    }
}
