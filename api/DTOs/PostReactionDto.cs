namespace api.DTOs
{
    public class PostReactionDto
    {
        public int PostId { get; set; }
        public int UserId { get; set; }
        public string ReactionType { get; set; }
    }
}
