using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace MailMate_BE_V2.Models
{
    public class Email
    {
        [Key]
        public Guid EmailId { get; set; }

        [Required]
        public Guid EmailAccountId { get; set; }

        [StringLength(255)]
        public string Subject { get; set; }

        public string Body { get; set; }

        public string Summary { get; set; }

        [Required]
        public bool IsSpam { get; set; }

        [Required]
        public DateTime ReceivedAt { get; set; }

        // Navigation properties
        [ForeignKey("EmailAccountId")]
        public virtual EmailAccount EmailAccount { get; set; }

        public virtual ICollection<EmailTagMapping> EmailTagMappings { get; set; }
    }
}
