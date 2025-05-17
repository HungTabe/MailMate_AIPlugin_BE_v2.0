using MailMate_BE_V2.Interfaces;
using MailMate_BE_V2.Models.PayOS;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Net.payOS.Types;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace MailMate_BE_V2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentService _paymentService;

        public PaymentController(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        [HttpPost("create-payment")]
        [Authorize]
        public async Task<IActionResult> CreatePayment([FromBody] PaymentRequestDto request)
        {
            var userId = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value
                    ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            // Chuyển đổi userId sang Guid
            if (!Guid.TryParse(userId, out var parsedUserId))
            {
                return BadRequest("UserId không hợp lệ.");
            }

            // Gán userId vào request
            request.UserId = parsedUserId;
            var response = await _paymentService.CreatePaymentLinkAsync(request);
            return Ok(response);
        }

        [HttpGet("status/{transactionId}")]
        public async Task<IActionResult> GetPaymentStatus(string transactionId)
        {
            var payment = await _paymentService.GetPaymentStatusAsync(transactionId);
            return Ok(payment);
        }

        [HttpPost("webhook")]
        public async Task<IActionResult> HandleWebhook([FromBody] WebhookType webhookData)
        {
            var result = await _paymentService.HandleWebhookAsync(webhookData);
            return result ? Ok() : BadRequest("Xử lý webhook thất bại.");
        }
    }
}
