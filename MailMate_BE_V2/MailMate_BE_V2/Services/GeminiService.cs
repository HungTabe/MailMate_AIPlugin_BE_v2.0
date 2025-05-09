using MailMate_BE_V2.DTOs.Gemini;
using MailMate_BE_V2.Interfaces;
using Microsoft.Extensions.Options;

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

        public async Task<string> GeminiSummarizeTextAsync(string text, int maxLength)
        {
            // Tạo prompt yêu cầu tóm tắt
            var prompt = $"Tóm tắt đoạn văn bản sau thành khoảng {maxLength} từ, giữ nguyên ý chính:\n\n{text} . Và bên cạnh đó hãy phân tích thành nội dung sau";

            // Tạo request body theo mẫu cURL
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

            // Gửi yêu cầu đến Gemini API
            var url = $"{_settings.Endpoint}?key={_settings.ApiKey}";
            var response = await _httpClient.PostAsJsonAsync(url, requestBody);

            // Kiểm tra lỗi
            response.EnsureSuccessStatusCode();

            // Ánh xạ phản hồi
            var result = await response.Content.ReadFromJsonAsync<GeminiResponse>();
            var summary = result?.Candidates?.FirstOrDefault()?.Content?.Parts?.FirstOrDefault()?.Text;

            return string.IsNullOrEmpty(summary) ? "Không thể tóm tắt văn bản." : summary;
        }
    }
}
