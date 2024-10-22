using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace api.Models
{
    [Table("two_fa_tokens")]
    public class TwoFaToken
    {
        [Key]
        [Column("token_id")]
        public int TokenId { get; set; }

        [Column("user_id")]
        public int UserId { get; set; }

        [Column("token")]
        [StringLength(6)]
        public string Token { get; set; }

        [Column("expires_at")]
        public DateTime ExpiresAt { get; set; }
    }
}
