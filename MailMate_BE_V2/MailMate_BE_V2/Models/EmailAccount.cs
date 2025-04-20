using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace MailMate_BE_V2.Models
{
    public class EmailAccount
    {
        [Key]
        public Guid EmailAccountId { get; set; }

        [Required]
        public Guid UserId { get; set; }

        [Required]
        [StringLength(50)]
        public string Provider { get; set; }

        [Required]
        [StringLength(500)]
        public string AccessToken { get; set; }

        [StringLength(500)]
        public string RefreshToken { get; set; }

        [Required]
        public DateTime ConnectedAt { get; set; }

        // Navigation properties
        [ForeignKey("UserId")]
        public virtual User User { get; set; }

        public virtual ICollection<Email> Emails { get; set; }
        public virtual ICollection<EmailSchedule> EmailSchedules { get; set; }
    }
}
