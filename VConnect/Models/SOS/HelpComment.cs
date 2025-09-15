using System;
using System.ComponentModel.DataAnnotations;
using VConnect.Models; // ApplicationUser

namespace VConnect.Models.SOS
{
    public class HelpComment
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int HelpRequestId { get; set; }
        public HelpRequest HelpRequest { get; set; }

        [Required]
        public int UserId { get; set; }
        public ApplicationUser User { get; set; }

        [Required, MaxLength(3000)]
        public string Message { get; set; }

        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

        public bool IsEdited { get; set; } = false;
        public DateTimeOffset? EditedAt { get; set; }
        public bool IsDeleted { get; set; } = false;
    }
}
