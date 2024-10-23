﻿using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace api.Models
{
    [Table("friends")]
    public class Friend
    {
        [Key]
        [Column("friend_id")]
        public int FriendId { get; set; }

        [Column("user_id_1")]
        public int UserId1 { get; set; }

        [Column("user_id_2")]
        public int UserId2 { get; set; }

        [Column("status")]
        [StringLength(20)]
        public string Status { get; set; }

        [Column("requested_at")]
        public DateTime RequestedAt { get; set; }
    }
}