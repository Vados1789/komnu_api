﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace api.Models
{
    [Table("users")]
    public class User
    {
        [Key]
        [Column("user_id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)] // Auto-incrementing ID
        public int UserId { get; set; }

        [Column("username")]
        [StringLength(50)]
        [Required] // Username must be provided
        public string Username { get; set; }

        [Column("email")]
        [StringLength(100)]
        [Required] // Email must be provided
        public string Email { get; set; }

        [Column("phone_number")]
        [StringLength(20)]
        public string PhoneNumber { get; set; } // Optional

        [Column("profile_picture")]
        [StringLength(255)]
        public string ProfilePicture { get; set; } // Optional

        [Column("bio")]
        public string Bio { get; set; } // Optional

        [Column("date_of_birth")]
        public DateTime? DateOfBirth { get; set; } // Now optional

        [Column("created_at")]
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)] // Automatically generated by DB
        public DateTime CreatedAt { get; set; }

        // Navigation property for Comments
        public ICollection<Comment> Comments { get; set; } = new List<Comment>();
        public ICollection<PostReaction> PostReactions { get; set; } = new List<PostReaction>();
        public ICollection<GroupMember> GroupMemberships { get; set; } = new List<GroupMember>(); // Groups user is part of
        public ICollection<GroupPost> GroupPosts { get; set; } = new List<GroupPost>(); // Posts made by the user in groups
        public ICollection<GroupComment> GroupComments { get; set; } = new List<GroupComment>(); // Comments in groups
        public ICollection<GroupReaction> GroupReactions { get; set; } = new List<GroupReaction>(); // Reactions in groups
    }
}
