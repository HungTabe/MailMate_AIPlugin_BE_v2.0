using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace MailMate_BE_V2.Models
{
    public class Log
    {
        [Key]
        public Guid LogId { get; set; }

        [Required]
        public Guid UserId { get; set; }

        [Required]
        [StringLength(100)]
        public string Action { get; set; }

        [Required]
        public DateTime Timestamp { get; set; }

        public string Metadata { get; set; } // JSON

        // Navigation properties
        [ForeignKey("UserId")]
        public virtual User User { get; set; }
    }
}
