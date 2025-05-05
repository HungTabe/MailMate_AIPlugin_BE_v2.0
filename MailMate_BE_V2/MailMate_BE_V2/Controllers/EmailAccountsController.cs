using MailMate_BE_V2.DTOs;
using MailMate_BE_V2.Services;
using Microsoft.AspNetCore.Mvc;

namespace MailMate_BE_V2.Controllers
{
    [Route("api/email-accounts")]
    [ApiController]
    public class EmailAccountsController : ControllerBase
    {
        private readonly IEmailAccountService _emailAccountService;

        public EmailAccountsController(IEmailAccountService emailAccountService)
        {
            _emailAccountService = emailAccountService;
        }

        [HttpPost("connect")] //- Kết nối tài khoản email qua OAuth
        public async Task<ActionResult<EmailAccountConnectGoogleResponse>> ConnectEmailAccount([FromBody] EmailAccountConnectGoogleRequest request)
        {
            var response = await _emailAccountService.ConnectGoogleEmailAccountAsync(request.AuthorizationCode);
            return Ok(response);
        }

        [HttpGet] //- Lấy danh sách tài khoản email đã kết nối
        public async Task<ActionResult<List<EmailAccountListResponse>>> GetEmailAccounts()
        {
            var response = await _emailAccountService.GetEmailAccountsAsync();
            return Ok(response);
        }

        [HttpGet("{id}")] //- Lấy chi tiết tài khoản email
        public async Task<ActionResult<EmailAccountDetailResponse>> GetEmailAccountById(Guid id)
        {
            var response = await _emailAccountService.GetEmailAccountByIdAsync(id);
            return Ok(response);
        }
        [HttpDelete("{id}")] //- ngắt kết nối tài khoản email
        public async Task<IActionResult> DeleteEmailAccount(Guid id)
        {
            await _emailAccountService.DeleteEmailAccountAsync(id);
            return NoContent();
        }
    }
}
