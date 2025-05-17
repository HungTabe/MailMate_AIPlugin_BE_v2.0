using MailMate_BE_V2.Models.PayOS;
using MailMate_BE_V2.Models;
using Net.payOS.Types;

namespace MailMate_BE_V2.Interfaces
{
    public interface IPaymentService
    {
        Task<PaymentResponseDto> CreatePaymentLinkAsync(PaymentRequestDto request);
        Task<Payment> GetPaymentStatusAsync(string transactionId);
        Task<bool> HandleWebhookAsync(WebhookType webhookData);
    }
}
