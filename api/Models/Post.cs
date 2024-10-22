using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace api.Models
{
    [Table("posts")] // Map to 'posts' table
    public class Post
    {
        [Key]
        [Column("post_id")] // Map 'PostId' to 'post_id'
        public int PostId { get; set; }

        [Column("user_id")] // Map 'UserId' to 'user_id'
        public int UserId { get; set; }

        [Column("content")] // Map 'Content' to 'content'
        public string Content { get; set; }

        [Column("image_path")] // Map 'ImagePath' to 'image_path'
        public string ImagePath { get; set; }

        [Column("created_at")] // Map 'CreatedAt' to 'created_at'
        public DateTime CreatedAt { get; set; }

        //public User User { get; set; }
        //public ICollection<Comment> Comments { get; set; }
    }
}
