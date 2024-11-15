using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace api.Models
{
    [Table("groups")]
    public class Group
    {
        [Key]
        [Column("group_id")]
        public int GroupId { get; set; }

        [Column("group_name")]
        [StringLength(100)]
        public string GroupName { get; set; }

        [Column("description")]
        public string Description { get; set; }

        [Column("image_url")]
        public string ImageUrl { get; set; }

        // New field for the creator
        [Column("creator_user_id")]
        public int CreatorUserId { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation property for the creator (User)
        public User Creator { get; set; }

        // Navigation properties for relationships
        public ICollection<GroupMember> Members { get; set; } = new List<GroupMember>();
        public ICollection<GroupPost> GroupPosts { get; set; } = new List<GroupPost>();
        public ICollection<GroupComment> GroupComments { get; set; } = new List<GroupComment>();
    }
}
