using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace VConnect.Models.SOS
{
    public class SosPost
    {
        public int Id { get; set; }

        // Creator (nullable for guests)
        public string? OwnerUserId { get; set; }
        public bool IsGuest { get; set; }

        // Form fields (match your modal)
        [MaxLength(100)]
        public string Name { get; set; }           // Your Name (optional if logged-in)
        [MaxLength(150)]
        public string Contact { get; set; }        // Email 
        [MaxLength(300)]
        public string Location { get; set; }       // Free-text location
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }

        [MaxLength(50)]
        public string EmergencyType { get; set; }  // "fire", "accident", "flood", "medical", "other"

        [MaxLength(2000)]
        public string Description { get; set; }

        // Feed controls
        [MaxLength(50)]
        public string Status { get; set; } = "Urgent"; // Urgent | Active | Completed
        public bool IsAcceptingHelp { get; set; } = true;
        public bool IsVisible { get; set; } = true;
        public bool IsDeleted { get; set; } = false;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? CompletedAt { get; set; }

        // Navigation
        public ICollection<SosComment> Comments { get; set; } = new List<SosComment>();
    }
}
