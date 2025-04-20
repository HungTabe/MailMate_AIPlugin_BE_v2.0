using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace MailMate_BE_V2.Models
{
    public class AutoReplyScript
    {
        [Key]
        public Guid ScriptId { get; set; }

        [Required]
        public Guid UserId { get; set; }

        [StringLength(100)]
        public string TriggerKeyword { get; set; }

        public string Conditions { get; set; } // JSON

        [Required]
        public string ReplyContent { get; set; }

        public int? SequenceOrder { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; }

        // Navigation properties
        [ForeignKey("UserId")]
        public virtual User User { get; set; }
    }
}
