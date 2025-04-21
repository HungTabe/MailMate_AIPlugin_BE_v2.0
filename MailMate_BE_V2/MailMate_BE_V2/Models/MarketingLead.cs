using MailMate_BE_V2.Data.EnumData;
using System.ComponentModel.DataAnnotations;

namespace MailMate_BE_V2.Models
{
    public class MarketingLead
    {
        [Key]
        public Guid LeadId { get; set; }

        [Required]
        [StringLength(100)]
        public string FullName { get; set; }

        [Required]
        [StringLength(255)]
        public string Email { get; set; }

        [StringLength(20)]
        public string PhoneNumber { get; set; }

        [Required]
        public SubscriptionPlan PlanType { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; }

        [Required]
        public LeadStatus Status { get; set; }
    }
}
