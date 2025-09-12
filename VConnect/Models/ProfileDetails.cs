using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Http;

namespace VConnect.Models
{
    public class ProfileDetails
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        // First and Last name
        [Required, StringLength(50)]
        [Display(Name = "First Name")]
        public string FirstName { get; set; } = "";

        [Required, StringLength(50)]
        [Display(Name = "Last Name")]
        public string LastName { get; set; } = "";

        // Email (unique)
        [Required, EmailAddress]
        [StringLength(100)]
        [Display(Name = "Email")]
        public string Email { get; set; } = "";

        // Date of Birth
        [DataType(DataType.Date)]
        [Display(Name = "Date of Birth")]
        public DateTime? DateOfBirth { get; set; }

        // Phone
        [Phone]
        [Display(Name = "Phone Number")]
        public string? PhoneNumber { get; set; }

        // City
        [StringLength(100)]
        public string? City { get; set; }

        // Address
        [StringLength(200)]
        public string? Address { get; set; }

        // Avatar/Profile picture path
        [Display(Name = "Profile Picture")]
        public string? ProfilePictureUrl { get; set; }

        // Not mapped field for upload
        [NotMapped]
        [Display(Name = "Upload New Picture")]
        public IFormFile? ProfilePictureFile { get; set; }

        // --- Read-only fields (updated only from DB/business logic) ---
        [Display(Name = "Total Volunteer Work Hours")]
        public int TotalVolunteerHours { get; set; }

        [Display(Name = "Events Participated")]
        public int EventsParticipated { get; set; }

        // Audit fields
        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAtUtc { get; set; }
    }
}
