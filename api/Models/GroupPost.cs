using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace api.Models
{
    [Table("group_posts")]
    public class GroupPost
    {
        [Key]
        [Column("post_id")]
        public int PostId { get; set; }

        [Column("group_id")]
        public int GroupId { get; set; }

        [Column("user_id")]
        public int UserId { get; set; }

        [Column("content")]
        public string Content { get; set; }

        [Column("image_path")]
        public string ImagePath { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public Group Group { get; set; }
        public User User { get; set; }
        public ICollection<GroupComment> Comments { get; set; } = new List<GroupComment>();
        public ICollection<GroupReaction> Reactions { get; set; } = new List<GroupReaction>();
    }
}
