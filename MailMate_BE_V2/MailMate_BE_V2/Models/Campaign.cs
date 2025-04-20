using MailMate_BE_V2.Data.EnumData;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace MailMate_BE_V2.Models
{
    public class Campaign
    {
        [Key]
        public Guid CampaignId { get; set; }

        [Required]
        public Guid UserId { get; set; }

        [Required]
        [StringLength(100)]
        public string Title { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; }

        [Required]
        public CampaignStatus Status { get; set; }

        // Navigation properties
        [ForeignKey("UserId")]
        public virtual User User { get; set; }

        public virtual ICollection<CampaignRecipient> CampaignRecipients { get; set; }
    }
}
