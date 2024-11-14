using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace api.Models
{
    [Table("group_members")]
    public class GroupMember
    {
        [Key]
        [Column("group_member_id")]
        public int GroupMemberId { get; set; }

        [Column("group_id")]
        public int GroupId { get; set; }

        [Column("user_id")]
        public int UserId { get; set; }

        [Column("joined_at")]
        public DateTime JoinedAt { get; set; } = DateTime.Now;

        public Group Group { get; set; }
        public User User { get; set; }
    }
}
