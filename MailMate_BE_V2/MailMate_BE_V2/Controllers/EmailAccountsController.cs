using MailMate_BE_V2.DTOs;
using MailMate_BE_V2.Interfaces;
using MailMate_BE_V2.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;

namespace MailMate_BE_V2.Controllers
{
    [Route("api/email-accounts")]
    [ApiController]
    [Authorize] // Yêu cầu người dùng đăng nhập
    public class EmailAccountsController : ControllerBase
    {
        private readonly IEmailAccountService _emailAccountService;

        public EmailAccountsController(IEmailAccountService emailAccountService)
        {
            _emailAccountService = emailAccountService;
        }

        [HttpGet("auth-url")]
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

        [HttpGet("email-account-detail")]
        public async Task<ActionResult<EmailAccountDetailResponse>> GetEmailAccountById(Guid id)
        {
            try
            {
                var response = await _emailAccountService.GetEmailAccountByIdAsync(id);
                return Ok(response);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { Message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return Unauthorized(new { Message = ex.Message });
            }
            catch
            {
                return StatusCode(500, new { Message = "An error occurred while fetching the email account." });
            }
        }

        [HttpDelete("delete-email-account")]
        public async Task<IActionResult> DeleteEmailAccount(Guid id)
        {
            try
            {
                await _emailAccountService.DeleteEmailAccountAsync(id);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { Message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return Unauthorized(new { Message = ex.Message });
            }
            catch
            {
                return StatusCode(500, new { Message = "An error occurred while deleting the email account." });
            }
        }
    }
}