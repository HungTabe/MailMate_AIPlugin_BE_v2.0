using MailMate_BE_V2.Data.EnumData;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace MailMate_BE_V2.Models
{
    public class EmailSchedule
    {
        [Key]
        public Guid ScheduleId { get; set; }

        [Required]
        public Guid EmailAccountId { get; set; }

        [Required]
        [StringLength(255)]
        public string ToEmail { get; set; }

        [StringLength(255)]
        public string Subject { get; set; }

        public string Body { get; set; }

        [Required]
        public DateTime ScheduledAt { get; set; }

        [Required]
        public ScheduleStatus Status { get; set; }

        // Navigation properties
        [ForeignKey("EmailAccountId")]
        public virtual EmailAccount EmailAccount { get; set; }
    }
}
