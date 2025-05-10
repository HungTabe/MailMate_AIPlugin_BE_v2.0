using MailMate_BE_V2.DTOs;
using MailMate_BE_V2.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace MailMate_BE_V2.Controllers
{
    [ApiController]
    [Route("api/emails/inbox")]
    [Authorize]
    public class EmailsController : ControllerBase
    {
        private readonly IEmailService _emailService;

        public EmailsController(IEmailService emailService)
        {
            _emailService = emailService;
        }

        [HttpGet("top-10-with-AISummarize")]
        public async Task<IActionResult> GetInboxEmails()
        {
            try
            {
                // Kiểm tra xác thực
                if (!User.Identity.IsAuthenticated)
                {
                    return Unauthorized("User is not authenticated.");
                }

                var userIdClaim = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value
                    ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdClaim))
                {
                    return Unauthorized("User ID claim is missing.");
                }

                // Chuyển string thành Guid
                if (!Guid.TryParse(userIdClaim, out var userId))
                {
                    return Unauthorized("Invalid user ID format.");
                }

                // Gọi service với userId kiểu Guid
                var emails = await _emailService.GetInboxEmailsAsync(userId);
                return Ok(new { Emails = emails });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Error = "Internal server error: " + ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<EmailDto>> GetEmail(string id)
        {
            try
            {
                var userId = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value
                    ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized(new { message = "Invalid user" });
                }

                var email = await _emailService.GetEmailByIdAsync(id, userId);
                return Ok(email);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while fetching email", details = ex.Message });
            }
        }
    }
}
