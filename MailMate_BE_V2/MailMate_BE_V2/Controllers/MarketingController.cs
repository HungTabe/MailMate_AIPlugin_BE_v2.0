using MailMate_BE_V2.DTOs;
using MailMate_BE_V2.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace MailMate_BE_V2.Controllers
{
    [Route("api/marketing")]
    [ApiController]
    public class MarketingController : ControllerBase
    {
        private readonly IMarketingService _marketingService;

        public MarketingController(IMarketingService marketingService)
        {
            _marketingService = marketingService;
        }

        [HttpPost("consultation-request")]
        public async Task<IActionResult> SubmitConsultationRequest([FromBody] ConsultationRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { message = "Invalid email format, phone number format, or missing required fields" });
            }

            try
            {
                await _marketingService.SubmitConsultationRequestAsync(request);
                return Ok(new { message = "Consultation request submitted successfully. You will receive an email soon." });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while processing your request" });
            }
        }
    }
}
