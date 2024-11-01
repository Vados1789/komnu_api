using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace api.Models
{
    [Table("posts")]
    public class Post
    {
        [Key]
        [Column("post_id")]
        public int PostId { get; set; }

        [Column("user_id")]
        public int UserId { get; set; }

        [Column("content")]
        public string Content { get; set; }

        [Column("image_path")]
        public string ImagePath { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; }

        // Add the User navigation property
        [ForeignKey("UserId")]
        public User User { get; set; }
    }
}
