using MailMate_BE_V2.DTOs;
using MailMate_BE_V2.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

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

        [HttpGet("auth-url")] //GET   /api/email-accounts/connect        - Kết nối tài khoản email qua OAuth
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
        public async Task<ActionResult<List<EmailAccountListResponse>>> GetEmailAccounts()
        {
            try
            {
                var response = await _emailAccountService.GetEmailAccountsAsync();
                return Ok(response);
            }
            catch (InvalidOperationException ex)
            {
                return Unauthorized(new { Message = ex.Message });
            }
            catch
            {
                return StatusCode(500, new { Message = "An error occurred while fetching email accounts." });
            }
        }

    }
}
