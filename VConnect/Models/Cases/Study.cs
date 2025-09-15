using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace VConnect.Models.Cases
{
    public class Study
    {
        [Key]
        public int Id { get; set; }

        [Required, MaxLength(200)]
        public string Title { get; set; }

        [MaxLength(300)]
        public string Subtitle { get; set; }

        [MaxLength(500)]
        public string ImageUrl { get; set; }

        [Required, MaxLength(100)]
        public string Category { get; set; }

        [MaxLength(150)]
        public string Location { get; set; }

        [MaxLength(100)]
        public string Duration { get; set; }

        public int Volunteers { get; set; }
        public int Beneficiaries { get; set; }

        [MaxLength(1000)]
        public string ShortDescription { get; set; }

        public string Challenge { get; set; }
        public string Solution { get; set; }
        public string Results { get; set; }

        [MaxLength(50)]
        public string Status { get; set; }

        [MaxLength(100)]
        public string Budget { get; set; }

        [MaxLength(300)]
        public string Partners { get; set; }

        // Navigation properties
        public ICollection<CaseGalleryImage> GalleryImages { get; set; } = new List<CaseGalleryImage>();
        public ICollection<Milestone> Milestones { get; set; } = new List<Milestone>();
    }
}
