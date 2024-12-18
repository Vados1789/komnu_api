﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace api.Models
{
    [Table("comments")]
    public class Comment
    {
        [Key]
        [Column("comment_id")]
        public int CommentId { get; set; }

        [Column("post_id")]
        public int PostId { get; set; }

        [Column("user_id")]
        public int UserId { get; set; }

        [Column("parent_comment_id")]
        public int? ParentCommentId { get; set; } // Nullable to allow top-level comments

        [Column("content")]
        public string Content { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; }

        // Navigation properties
        public Post Post { get; set; }
        public User User { get; set; }
        public Comment ParentComment { get; set; } // Reference to parent comment
        public ICollection<Comment> Replies { get; set; } = new List<Comment>(); // Collection for replies
    }
}
