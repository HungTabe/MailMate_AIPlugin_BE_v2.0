using MailMate_BE_V2.DTOs.Gemini;
using MailMate_BE_V2.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MailMate_BE_V2.Controllers
{
    [Route("api/gemini")]
    [ApiController]
    public class GeminiController : ControllerBase
    {
        private readonly IGeminiService _geminiService;

        public GeminiController(IGeminiService geminiService)
        {
            _geminiService = geminiService;
        }

        [HttpPost("summarize")]
        public async Task<IActionResult> Summarize([FromBody] GeminiSummarizeRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Text))
            {
                return BadRequest("Văn bản không được để trống.");
            }

            if (request.MaxLength <= 0)
            {
                return BadRequest("Độ dài tóm tắt phải lớn hơn 0.");
            }

            try
            {
                var summary = await _geminiService.GeminiSummarizeTextAsync(request.Text, request.MaxLength);
                return Ok(new SummarizeResponse { Summary = summary });
            }
            catch (HttpRequestException ex)
            {
                return StatusCode(500, $"Lỗi khi gọi Gemini API: {ex.Message}");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi không xác định: {ex.Message}");
            }
        }
    }
}
