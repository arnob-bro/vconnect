using System;
using System.ComponentModel.DataAnnotations;

namespace VConnect.Models.Cases
{
    public class Milestone
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public DateTime Date { get; set; }

        [Required, MaxLength(200)]
        public string Title { get; set; }

        public string Description { get; set; }

        // Foreign Key
        public int StudyId { get; set; }

        // Navigation Property
        public Study Study { get; set; }
    }
}
