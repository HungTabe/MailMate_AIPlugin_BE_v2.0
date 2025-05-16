using MailMate_BE_V2.Data; // Thêm dòng này để sử dụng MailMateDbContext
using MailMate_BE_V2.Data.EnumData;
using MailMate_BE_V2.DTOs;
using MailMate_BE_V2.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore; // Thêm dòng này để sử dụng FirstOrDefaultAsync
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;


namespace MailMate_BE_V2.Controllers

{
    [Route("api/email-accounts")]
    [ApiController]
    public class EmailAccountsController : ControllerBase
    {
        private readonly IEmailAccountService _emailAccountService;
        private readonly MailMateDbContext _context; // Thêm dòng này để sử dụng MailMateDbContext
        public EmailAccountsController(IEmailAccountService emailAccountService, MailMateDbContext context)
        {
            _emailAccountService = emailAccountService;
            _context = context; // Inject MailMateDbContext vào đây
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

        /*[HttpGet("list")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<List<EmailAccountListResponse>>> GetEmailAccounts()
        {
            var userIdResult = GetUserId();
            if (!userIdResult.IsSuccess)
            {
                return Unauthorized(new { Success = false, Message = userIdResult.ErrorMessage });
            }

            try
            {
                // Kiểm tra quyền admin
                var user = await _context.Users
                    .FirstOrDefaultAsync(u => u.UserId == userIdResult.UserId);
                if (user == null)
                {
                    return NotFound(new { Success = false, Message = "Người dùng không tồn tại." });
                }

                if (user.Role != UserRole.Admin) // Sửa từ "Admin" thành UserRole.Admin
                {
                    return StatusCode(403, new { Success = false, Message = "Bạn không có quyền truy cập API này." });
                }

                // Admin: Lấy danh sách tất cả tài khoản email
                var response = await _emailAccountService.GetAllEmailAccountsAsync();
                return Ok(new { Success = true, Data = response });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Success = false, Message = $"Đã xảy ra lỗi khi lấy danh sách tài khoản email: {ex.Message}" });
            }
        }*/
        [HttpGet("list")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<List<EmailAccountListResponse>>> GetEmailAccounts()
        {
            var userIdResult = GetUserId();
            if (!userIdResult.IsSuccess)
            {
                return Unauthorized(new { Success = false, Message = userIdResult.ErrorMessage });
            }

            try
            {
                // Không cần kiểm tra quyền admin nữa, vì [Authorize(Roles = "Admin")] đã xử lý
                var response = await _emailAccountService.GetAllEmailAccountsAsync();
                return Ok(new { Success = true, Data = response });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Success = false, Message = $"Đã xảy ra lỗi khi lấy danh sách tài khoản email: {ex.Message}" });
            }
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteEmailAccount(Guid id)
        {
            var userIdResult = GetUserId();
            if (!userIdResult.IsSuccess)
            {
                return Unauthorized(new { Success = false, Message = userIdResult.ErrorMessage });
            }

            try
            {
                await _emailAccountService.DeleteEmailAccountAsync(userIdResult.UserId, id);
                return Ok(new { Success = true, Message = "Tài khoản email đã được xóa." });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { Success = false, Message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Success = false, Message = $"Đã xảy ra lỗi khi xóa tài khoản email: {ex.Message}" });
            }
        }

        private (bool IsSuccess, Guid UserId, string ErrorMessage) GetUserId()
        {
            var userIdClaim = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value
                ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
            {
                return (false, Guid.Empty, "Không thể lấy userId từ token JWT.");
            }

            return (true, userId, null);
        }
    }
}