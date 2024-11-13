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

        [Column("created_at")]
        public DateTime CreatedAt { get; set; }

        // Navigation properties for relationships
        public ICollection<GroupMember> Members { get; set; } = new List<GroupMember>(); // Group members
        public ICollection<GroupPost> GroupPosts { get; set; } = new List<GroupPost>(); // Posts made in this group
        public ICollection<GroupComment> GroupComments { get; set; } = new List<GroupComment>(); // Comments in this group
        public ICollection<GroupReaction> GroupReactions { get; set; } = new List<GroupReaction>(); // Reactions to posts/comments in this group

        // Optional: This can track the group owner/creator if needed
        [ForeignKey("CreatorUserId")]
        public int? CreatorUserId { get; set; } // Optional, if you want to track the creator/owner
        public User Creator { get; set; } // Navigation property to the user who created the group
    }
}
