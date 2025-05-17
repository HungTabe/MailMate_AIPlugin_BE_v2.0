using MailMate_BE_V2.Data.EnumData;

namespace MailMate_BE_V2.Models.PayOS
{
    public class PaymentRequestDto
    {
        public Guid UserId { get; set; }
        public SubscriptionPlan PlanType { get; set; }
        public decimal Amount { get; set; }
        public string ReturnUrl { get; set; } // URL trả về sau khi thanh toán
        public string CancelUrl { get; set; } // URL khi hủy thanh toán
    }
}
