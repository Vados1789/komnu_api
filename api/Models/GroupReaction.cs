using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace api.Models
{
    [Table("group_reactions")]
    public class GroupReaction
    {
        [Key]
        [Column("reaction_id")]
        public int ReactionId { get; set; }

        [Column("post_id")]
        public int? PostId { get; set; }  // Nullable if reaction is on a comment

        [Column("comment_id")]
        public int? CommentId { get; set; }  // Nullable if reaction is on a post

        [Column("user_id")]
        public int UserId { get; set; }

        [Column("reaction_type")]
        [StringLength(10)]
        public string ReactionType { get; set; }  // 'like' or 'dislike'

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public GroupPost Post { get; set; }
        public GroupComment Comment { get; set; }
        public User User { get; set; }
    }
}
