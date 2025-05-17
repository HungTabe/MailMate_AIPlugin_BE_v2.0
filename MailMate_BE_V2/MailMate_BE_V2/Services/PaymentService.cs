using MailMate_BE_V2.Data;
using MailMate_BE_V2.Data.EnumData;
using MailMate_BE_V2.Interfaces;
using MailMate_BE_V2.Models;
using MailMate_BE_V2.Models.PayOS;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Net.payOS;
using Net.payOS.Types;

namespace MailMate_BE_V2.Services
{
    public class PaymentService : IPaymentService
    {

        private readonly PayOS _payOS;
        private readonly MailMateDbContext _context;
        private readonly PayOSConfig _config;

        public PaymentService(IOptions<PayOSConfig> config, MailMateDbContext context)
        {
            _config = config.Value;
            _payOS = new PayOS(_config.ClientId, _config.ApiKey, _config.ChecksumKey);
            _context = context;
        }

        public async Task<PaymentResponseDto> CreatePaymentLinkAsync(PaymentRequestDto request)
        {
            
            // Kiểm tra người dùng tồn tại
            var user = await _context.Users.FindAsync(request.UserId);
            if (user == null)
            {
                throw new ArgumentException($"Người dùng với UserId {request.UserId} không tồn tại.");
            }

            // Kiểm tra PlanType và Amount
            if (request.PlanType == SubscriptionPlan.Free)
            {
                throw new ArgumentException("Gói Free không yêu cầu thanh toán.");
            }
            if (request.Amount <= 0)
            {
                throw new ArgumentException("Số tiền phải lớn hơn 0.");
            }

            // Unique code for each order
            var orderCode = (long)(DateTime.UtcNow.Ticks / TimeSpan.TicksPerSecond);

            var items = new List<ItemData>
            {
                new ItemData($"Subscription MailMate : {request.PlanType}", 1, (int)(request.Amount * 1000)) // PayOS yêu cầu số tiền tính bằng VNĐ
            };

            // Tạo PaymentData
            var paymentData = new PaymentData(
                orderCode: orderCode,
                amount: (int)(request.Amount * 1000), // Chuyển đổi sang VNĐ
                description: $"Thanh toán {request.PlanType}",
                items: items,
                returnUrl: request.ReturnUrl,
                cancelUrl: request.CancelUrl
            );

            // Gọi PayOS để tạo link thanh toán
            CreatePaymentResult result = await _payOS.createPaymentLink(paymentData);

            // Lưu thông tin thanh toán vào cơ sở dữ liệu
            var payment = new Payment
            {
                PaymentId = Guid.NewGuid(),
                UserId = request.UserId,
                PlanType = request.PlanType,
                Amount = request.Amount,
                TransactionId = result.paymentLinkId,
                Status = PaymentStatus.Pending,
                CreatedAt = DateTime.UtcNow,
                ValidUntil = DateTime.UtcNow.AddMonths(1) // Ví dụ: Gói có hiệu lực 1 tháng
            };

            _context.Payments.Add(payment);
            await _context.SaveChangesAsync();

            return new PaymentResponseDto
            {
                CheckoutUrl = result.checkoutUrl,
                TransactionId = result.paymentLinkId,
                Status = PaymentStatus.Pending
            };
        }

        public async Task<Payment> GetPaymentStatusAsync(string transactionId)
        {
            var payment = await _context.Payments
                .FirstOrDefaultAsync(p => p.TransactionId == transactionId);

            if (payment == null)
            {
                throw new Exception("Không tìm thấy giao dịch.");
            }

            // Lấy thông tin từ PayOS
            PaymentLinkInformation info = await _payOS.getPaymentLinkInformation(long.Parse(transactionId));
            payment.Status = info.status == "PAID" ? PaymentStatus.Completed :
                             info.status == "CANCELLED" ? PaymentStatus.Failed : PaymentStatus.Pending;

            await _context.SaveChangesAsync();
            return payment;
        }

        public async Task<bool> HandleWebhookAsync(WebhookType webhookData)
        {
            try
            {
                // Xác minh webhook (phương thức đồng bộ)
                WebhookData verifiedData = _payOS.verifyPaymentWebhookData(webhookData);

                // Tìm giao dịch trong cơ sở dữ liệu
                var payment = await _context.Payments
                    .FirstOrDefaultAsync(p => p.TransactionId == verifiedData.orderCode.ToString());

                if (payment == null)
                {
                    return false;
                }

                // Lấy thông tin trạng thái từ PayOS
                PaymentLinkInformation info = await _payOS.getPaymentLinkInformation(verifiedData.orderCode);

                // Cập nhật trạng thái thanh toán
                payment.Status = info.status == "PAID" ? PaymentStatus.Completed :
                                 info.status == "CANCELLED" ? PaymentStatus.Failed : PaymentStatus.Pending;

                // Cập nhật SubscriptionPlan và SubscriptionEndDate của User
                if (payment.Status == PaymentStatus.Completed)
                {
                    var user = await _context.Users.FindAsync(payment.UserId);
                    if (user != null)
                    {
                        user.SubscriptionPlan = payment.PlanType;
                        user.SubscriptionEndDate = payment.ValidUntil;
                    }
                }

                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                // Ghi log lỗi vào bảng Log (tùy chọn)
                var log = new Log
                {
                    LogId = Guid.NewGuid(),
                    UserId = Guid.Empty, // Hoặc gán UserId nếu có
                    Action = "HandleWebhook",
                    Timestamp = DateTime.UtcNow,
                    Metadata = $"Lỗi xử lý webhook: {ex.Message}"
                };
                _context.Logs.Add(log);
                await _context.SaveChangesAsync();

                return false;
            }
        }
    }
}
