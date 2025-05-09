using MailMate_BE_V2.DTOs;
using MailMate_BE_V2.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MailMate_BE_V2.Controllers
{
    [Route("api/huggingface")]
    [ApiController]
    public class HuggingFaceController : ControllerBase
    {
        private readonly IHuggingFaceService _huggingFaceService;

        public HuggingFaceController(IHuggingFaceService huggingFaceService)
        {
            _huggingFaceService = huggingFaceService;
        }

        [HttpPost("summarize-text-by-AI")]
        public async Task<IActionResult> Summarize([FromBody] SummarizeRequest request)
        {
            try
            {
                var (summary, inputLength, outputLength, executionTimeMs, errorMessage) =  await _huggingFaceService.SummarizeAsync_demo(request);

                var response = new SummarizeResponse
                {
                    Summary = summary,
                    InputLength = inputLength,
                    OutputLength = outputLength,
                    ExecutionTimeMs = executionTimeMs,
                    ErrorMessage = errorMessage
                };



                return Ok(response);


            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Error = $"Internal server error: {ex.Message}" });
            }
        }
    }
}
