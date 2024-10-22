using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace api.Models
{
    [Table("messages")]
    public class Message
    {
        [Key]
        [Column("message_id")]
        public int MessageId { get; set; }

        [Column("sender_id")]
        public int SenderId { get; set; }

        [Column("receiver_id")]
        public int ReceiverId { get; set; }

        [Column("content")]
        public string Content { get; set; }

        [Column("sent_at")]
        public DateTime SentAt { get; set; }

        [Column("is_read")]
        public bool IsRead { get; set; }
    }
}
