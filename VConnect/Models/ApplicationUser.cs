using System.ComponentModel.DataAnnotations;

namespace VConnect.Models
{
    public class ApplicationUser
    {
        [Key]
        public int Id { get; set; } 

        [Required, EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }
        [Required]
        public string Role { get; set; } = "Volunteer";

        [Range(0, int.MaxValue)]
        public int? HoursVolunteered { get; set; } = 0;

    }
}
