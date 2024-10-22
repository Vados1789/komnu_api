using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace api.Models
{
    [Table("users")]
    public class User
    {
        [Key]
        [Column("user_id")]
        public int UserId { get; set; }

        [Column("username")]
        [StringLength(50)]
        public string Username { get; set; }

        [Column("email")]
        [StringLength(100)]
        public string Email { get; set; }

        [Column("profile_picture")]
        [StringLength(255)]
        public string ProfilePicture { get; set; }

        [Column("bio")]
        public string Bio { get; set; }

        [Column("date_of_birth")]
        public DateTime DateOfBirth { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; }

        //public ICollection<Post> Posts { get; set; }
        //public ICollection<Comment> Comments { get; set; }
    }
}
