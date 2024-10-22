using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace api.Models
{
    [Table("logins")]
    public class Login
    {
        [Key]
        [Column("login_id")]
        public int LoginId { get; set; }

        [Column("user_id")]
        public int UserId { get; set; }

        [Column("login_type")]
        [StringLength(20)]
        public string LoginType { get; set; }

        [Column("login_value")]
        [StringLength(100)]
        public string LoginValue { get; set; }

        [Column("password_hash")]
        [StringLength(255)]
        public string PasswordHash { get; set; }

        [Column("is_two_fa_enabled")]
        public bool IsTwoFaEnabled { get; set; }

        [Column("two_fa_method")]
        [StringLength(20)]
        public string TwoFaMethod { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; }

        [Column("last_login")]
        public DateTime LastLogin { get; set; }
    }
}
