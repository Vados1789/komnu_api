using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace api.Models
{
    public class GroupComment
    {
        [Key]
        [Column("comment_id")]
        public int CommentId { get; set; }

        [Column("post_id")]
        public int PostId { get; set; }

        [Column("user_id")]
        public int UserId { get; set; }

        [Column("parent_comment_id")]
        public int? ParentCommentId { get; set; }

        [Column("content")]
        public string Content { get; set; }

        [Column("image_path")]
        public string ImagePath { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public GroupPost Post { get; set; }
        public User User { get; set; }
        public GroupComment ParentComment { get; set; }
        public ICollection<GroupComment> Replies { get; set; } = new List<GroupComment>();
        public ICollection<GroupReaction> Reactions { get; set; } = new List<GroupReaction>();
    }
}
