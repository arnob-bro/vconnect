using System.ComponentModel.DataAnnotations;

namespace VConnect.Models
{
    public class ApplicationUser
    {
        [Key]
        public int Id { get; set; } // Primary key

        [Required, EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }
    }
}
