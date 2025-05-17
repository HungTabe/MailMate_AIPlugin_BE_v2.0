using MailMate_BE_V2.Data.EnumData;

namespace MailMate_BE_V2.Models.PayOS
{
    public class PaymentResponseDto
    {
        public string CheckoutUrl { get; set; }
        public string TransactionId { get; set; }
        public PaymentStatus Status { get; set; }
    }
}
