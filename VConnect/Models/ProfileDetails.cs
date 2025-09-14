using System;
using System.ComponentModel.DataAnnotations;

namespace VConnect.Models
{
    public class ProfileDetails
    {
        [Key]
        public int ProfileDetailsId { get; set; }   // Primary key

        public int UserId { get; set; }             // Foreign key → ApplicationUser.Id

        [MaxLength(50)]
        public string FirstName { get; set; } = string.Empty;

        [MaxLength(50)]
        public string LastName { get; set; } = string.Empty;

        [MaxLength(100)]
        public string Email { get; set; } = string.Empty;

        public DateOnly? DateOfBirth { get; set; }  // mapped to Postgres "date"

        [MaxLength(30)]
        public string? PhoneNumber { get; set; }

        [MaxLength(100)]
        public string? City { get; set; }

        [MaxLength(200)]
        public string? Address { get; set; }

        [MaxLength(500)]
        public string? ProfilePictureUrl { get; set; }

        public int TotalVolunteerHours { get; set; }
        public int EventsParticipated { get; set; }

        // PostgreSQL "timestamptz"
        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
        public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.UtcNow;
    }
}
