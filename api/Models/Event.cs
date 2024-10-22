using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace api.Models
{
    [Table("events")]
    public class Event
    {
        [Key]
        [Column("event_id")]
        public int EventId { get; set; }

        [Column("creator_id")]
        public int CreatorId { get; set; }

        [Column("event_name")]
        [StringLength(100)]
        public string EventName { get; set; }

        [Column("description")]
        public string Description { get; set; }

        [Column("event_date")]
        public DateTime EventDate { get; set; }

        [Column("location")]
        [StringLength(255)]
        public string Location { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; }

        //public User User { get; set; }
    }
}
