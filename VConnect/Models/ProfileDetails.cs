using System;

namespace VConnect.Models
{
    public class ProfileDetails
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public int ProfileDetailsId { get; set; }   // Primary key for ProfileDetails

        public int UserId { get; set; }             // Foreign key → ApplicationUser.Id

        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;

        public DateTime? DateOfBirth { get; set; }
        public string? PhoneNumber { get; set; }
        public string? City { get; set; }
        public string? Address { get; set; }

        public string? ProfilePictureUrl { get; set; }
        public int TotalVolunteerHours { get; set; }
        public int EventsParticipated { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
