using MailMate_BE_V2.DTOs;
using MailMate_BE_V2.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;

namespace MailMate_BE_V2.Controllers
{
    [Route("api/email-accounts")]
    [ApiController]
    //[Authorize] // Yêu cầu người dùng đăng nhập - Hien tai develop nen ko bat Autho
    public class EmailAccountsController : ControllerBase
    {
        private readonly IEmailAccountService _emailAccountService;

        public EmailAccountsController(IEmailAccountService emailAccountService)
        {
            _emailAccountService = emailAccountService;
        }

        [HttpGet("connect")]
        public async Task<ActionResult<AuthUrlResponse>> GetAuthUrl()
        {
            try
            {
                var response = await _emailAccountService.GetAuthUrlAsync();
                return Ok(response);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
            catch
            {
                return StatusCode(500, new { Message = "An error occurred while generating the auth URL." });
            }
        }

        [HttpGet("callback")]
        public async Task<IActionResult> OAuthCallback(string code)
        {
            try
            {
                var redirectUrl = await _emailAccountService.HandleOAuthCallbackAsync(code);
                return Redirect(redirectUrl);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
            catch
            {
                return StatusCode(500, new { Message = "An error occurred while processing the OAuth callback." });
            }
        }
        [HttpGet("list")]
        [Authorize]
        public async Task<ActionResult<List<EmailAccountListResponse>>> GetEmailAccounts()
        {
            // Lấy userId từ token
            var userIdClaim = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value
                ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            // Kiểm tra userId có hợp lệ không
            if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
            {
                return Unauthorized(new { Success = false, Message = "Không thể lấy userId từ token JWT." });
            }

            try
            {
                var response = await _emailAccountService.GetEmailAccountsAsync(userId);
                return Ok(new { Success = true, Data = response });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Success = false, Message = $"Đã xảy ra lỗi khi lấy danh sách tài khoản email: {ex.Message}" });
            }
        }
        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<EmailAccountDetailResponse>> GetEmailAccountById(Guid id)
        {
            // Lấy userId từ token
            var userIdClaim = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value
                ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
            {
                return Unauthorized(new { Success = false, Message = "Không thể lấy userId từ token JWT." });
            }

            try
            {
                var response = await _emailAccountService.GetEmailAccountByIdAsync(userId, id);
                return Ok(new { Success = true, Data = response });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { Success = false, Message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Success = false, Message = $"Đã xảy ra lỗi khi lấy chi tiết tài khoản email: {ex.Message}" });
            }
        }


    }
}
