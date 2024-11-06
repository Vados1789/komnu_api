using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace api.Models
{
    [Table("post_reactions")]
    public class PostReaction
    {
        [Key]
        [Column("reaction_id")]
        public int ReactionId { get; set; }

        [Column("post_id")]
        public int PostId { get; set; }

        [Column("user_id")]
        public int UserId { get; set; }

        [Column("reaction_type")]
        [Required]
        [StringLength(10)]
        public string ReactionType { get; set; } // Should be either "like" or "dislike"

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        [ForeignKey("PostId")]
        public Post Post { get; set; }

        [ForeignKey("UserId")]
        public User User { get; set; }
    }
}
