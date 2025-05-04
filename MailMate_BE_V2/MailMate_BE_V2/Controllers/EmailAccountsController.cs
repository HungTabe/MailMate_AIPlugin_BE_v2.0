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

        [HttpPost("connect")]
        public async Task<ActionResult<EmailAccountConnectGoogleResponse>> ConnectEmailAccount([FromBody] EmailAccountConnectGoogleRequest request)
        {
            var response = await _emailAccountService.ConnectGoogleEmailAccountAsync(request.AuthorizationCode);
            return Ok(response);
        }
    }
}