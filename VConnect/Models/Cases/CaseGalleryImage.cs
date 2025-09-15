using System.ComponentModel.DataAnnotations;

namespace VConnect.Models.Cases
{
    public class CaseGalleryImage
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string ImageUrl { get; set; }

        // Foreign Key
        public int StudyId { get; set; }

        // Navigation Property
        public Study Study { get; set; }
    }
}
