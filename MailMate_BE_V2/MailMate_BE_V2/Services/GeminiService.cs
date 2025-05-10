using MailMate_BE_V2.DTOs;
using MailMate_BE_V2.DTOs.Gemini;
using MailMate_BE_V2.Interfaces;
using Microsoft.Extensions.Options;
using System.Diagnostics;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace MailMate_BE_V2.Services
{
    public class GeminiService : IGeminiService
    {
        private readonly HttpClient _httpClient;
        private readonly GeminiSettings _settings;

        public GeminiService(HttpClient httpClient, IOptions<GeminiSettings> settings)
        {
            _httpClient = httpClient;
            _settings = settings.Value;
        }

        public async Task<SummaryResult> GeminiSummarizeTextAsync(string text, int maxLength)
        {
            var result = new SummaryResult
            {
                Summary = "Unable to generate summary.",
                InputLength = text.Length,
                OutputLength = 0,
                ExecutionTimeMs = 0,
                ErrorMessage = null
            };

            try
            {
                var stopwatch = Stopwatch.StartNew();
                var flattenedText = Regex.Replace(text, @"[-\u001F\u007F-\u009F]", "");
                var prompt = $"Tóm tắt đoạn văn bản sau thành khoảng {maxLength} từ, giữ nguyên ý chính :\n\n{flattenedText}.";

                var requestBody = new
                {
                    contents = new[]
                    {
                new
                {
                    parts = new[]
                    {
                        new { text = prompt }
                    }
                }
            }
                };

                var url = $"{_settings.Endpoint}?key={_settings.ApiKey}";
                var response = await _httpClient.PostAsJsonAsync(url, requestBody);
                response.EnsureSuccessStatusCode();

                var geminiResponse = await response.Content.ReadFromJsonAsync<GeminiResponse>();
                var summary = geminiResponse?.Candidates?.FirstOrDefault()?.Content?.Parts?.FirstOrDefault()?.Text;

                stopwatch.Stop();
                result.Summary = string.IsNullOrEmpty(summary) ? "Unable to generate summary." : summary;

                if (string.IsNullOrEmpty(flattenedText))
                {
                    result.Summary = "Nội dung mail đề cập đến các thông tin nhạy cảm như STK Ngân hàng, Hóa đơn, v..v.. Vui lòng truy cập Gmail để biết chi tiết";
                }

                result.OutputLength = result.Summary.Length;
                result.ExecutionTimeMs = stopwatch.ElapsedMilliseconds;
            }
            catch (Exception ex)
            {
                result.ErrorMessage = $"Failed to summarize: {ex.Message}";
            }

            return result;
        }
    }
}
