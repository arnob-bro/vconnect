using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using VConnect.Models; // ApplicationUser

namespace VConnect.Models.SOS
{
    public class HelpRequest
    {
        [Key]
        public int Id { get; set; }

        // Who created the post (user or volunteer)
        [Required]
        public int OwnerUserId { get; set; }
        public ApplicationUser Owner { get; set; }

        [Required, MaxLength(140)]
        public string Title { get; set; }

        [Required, MaxLength(4000)]
        public string Description { get; set; }

        [MaxLength(120)]
        public string Region { get; set; }

        public double? Latitude { get; set; }
        public double? Longitude { get; set; }

        // Main lifecycle
        [Required]
        public HelpStatus Status { get; set; } = HelpStatus.Urgent;

        // Owner-only quick toggle: accepting help or not, without marking Completed
        public bool IsAcceptingHelp { get; set; } = true;

        // Hide from public feed without deleting
        public bool IsVisible { get; set; } = true;

        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
        public DateTimeOffset? UpdatedAt { get; set; }
        public DateTimeOffset? ClosedAt { get; set; }
        public DateTimeOffset? ExpiresAt { get; set; }

        // Soft delete for moderation
        public bool IsDeleted { get; set; } = false;

        // Navigation
        public ICollection<HelpComment> Comments { get; set; } = new List<HelpComment>();
    }
}
