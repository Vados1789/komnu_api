namespace api.DTOs
{
    public class CommentDto
    {
        public int CommentId { get; set; }
        public int PostId { get; set; }
        public int UserId { get; set; }
        public string Content { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Username { get; set; }
        public string ProfileImagePath { get; set; }
        public int? ParentCommentId { get; set; } // For replies
        public List<CommentDto> Replies { get; set; } = new List<CommentDto>();
    }
}
