namespace VConnect.Services.DTOs
{
    public class CreateSosCommentDto
    {
        public int SosPostId { get; set; }   // Which post the comment belongs to
        public string Content { get; set; }  // The actual comment text
        public int? ParentCommentId { get; set; } // If it's a reply to another comment (optional)
    }
}
