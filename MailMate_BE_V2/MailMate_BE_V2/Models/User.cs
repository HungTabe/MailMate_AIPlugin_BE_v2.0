using MailMate_BE_V2.Data.EnumData;
using System.ComponentModel.DataAnnotations;

namespace MailMate_BE_V2.Models
{
    public class User
    {
        [Key]
        public Guid UserId { get; set; }

        [Required]
        [StringLength(255)]
        public string Email { get; set; }

        [Required]
        [StringLength(255)]
        public string PasswordHash { get; set; }

        [StringLength(100)]
        public string FullName { get; set; }

        [Required]
        public UserRole Role { get; set; }

        [Required]
        public bool IsActive { get; set; }

        [Required]
        public SubscriptionPlan SubscriptionPlan { get; set; }

        public DateTime? SubscriptionEndDate { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; }

        // Navigation properties
        public virtual ICollection<EmailAccount> EmailAccounts { get; set; }
        public virtual ICollection<Campaign> Campaigns { get; set; }
        public virtual ICollection<AutoReplyScript> AutoReplyScripts { get; set; }
        public virtual ICollection<Payment> Payments { get; set; }
        public virtual ICollection<Log> Logs { get; set; }
    }
}
