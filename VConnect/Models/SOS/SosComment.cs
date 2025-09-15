using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace VConnect.Models.SOS
{
    public class SosComment
    {
        public int Id { get; set; }

        // FK to post
        public int SosPostId { get; set; }
        public SosPost SosPost { get; set; }

        // Threading (nullable for top-level)
        public int? ParentCommentId { get; set; }
        public SosComment ParentComment { get; set; }
        public ICollection<SosComment> Replies { get; set; } = new List<SosComment>();

        // Who commented (nullable for guests)
        public string? UserId { get; set; }
        [MaxLength(100)]
        public string AuthorName { get; set; }

        [Required, MaxLength(1000)]
        public string Message { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public bool IsDeleted { get; set; } = false;
    }
}
