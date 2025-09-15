using System.ComponentModel.DataAnnotations;

namespace VConnect.Models.Cases
{
    public class Category
    {
        [Key]
        public int Id { get; set; }   // Primary Key

        [Required, MaxLength(100)]
        public string Name { get; set; }

        public string Icon { get; set; }

        public int Count { get; set; }

        public bool IsActive { get; set; }
    }
}
