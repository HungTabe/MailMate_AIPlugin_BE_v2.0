using MailMate_BE_V2.Data.EnumData;

namespace MailMate_BE_V2.DTOs
{
    public class UserProfileDto
    {
        public Guid UserId { get; set; }
        public string Email { get; set; }
        public string FullName { get; set; }
        public UserRole Role { get; set; }
        public bool IsActive { get; set; }
        public SubscriptionPlan SubscriptionPlan { get; set; }
        public DateTime? SubscriptionEndDate { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Provider { get; set; }
    }
}
