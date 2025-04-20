using MailMate_BE_V2.Data.EnumData;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace MailMate_BE_V2.Models
{
    public class CampaignRecipient
    {
        [Key]
        public Guid RecipientId { get; set; }

        [Required]
        public Guid CampaignId { get; set; }

        [Required]
        [StringLength(255)]
        public string Email { get; set; }

        [Required]
        public RecipientStatus Status { get; set; }

        public DateTime? SentAt { get; set; }

        public string ResponseMessage { get; set; }

        // Navigation properties
        [ForeignKey("CampaignId")]
        public virtual Campaign Campaign { get; set; }
    }
}
