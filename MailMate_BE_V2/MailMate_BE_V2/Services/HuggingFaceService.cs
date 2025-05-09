using MailMate_BE_V2.DTOs;
using MailMate_BE_V2.Interfaces;
using System.Diagnostics;
using System.Text;
using System.Text.Json;

namespace MailMate_BE_V2.Services
{
    public class HuggingFaceService : IHuggingFaceService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly ILogger<HuggingFaceService> _logger;


        public HuggingFaceService(HttpClient httpClient, IConfiguration configuration, ILogger<HuggingFaceService> logger)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<(string Summary, int InputLength, int OutputLength, int ExecutionTimeMs, string ErrorMessage)> SummarizeAsync(string text)
        {
            var apiKey = _configuration["HuggingFace:ApiKey"];
            var apiUrl = _configuration["HuggingFace:ApiUrl"];
            var stopwatch = Stopwatch.StartNew();

            try
            {
                // Giới hạn độ dài input để tránh vượt quota
                if (text.Length > 1000)
                {
                    text = text.Substring(0, 1000);
                }

                var requestBody = new
                {
                    inputs = text,
                    parameters = new
                    {
                        max_length = 200,
                        min_length = 50,
                        do_sample = false
                    }
                };

                var request = new HttpRequestMessage(HttpMethod.Post, apiUrl)
                {
                    Content = new StringContent(JsonSerializer.Serialize(requestBody))
                };

                // Đặt header Content-Type rõ ràng, không có charset
                request.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");

                // Đặt header Authorization
                request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", apiKey);

                // Ghi log header để debug
                Console.WriteLine($"Request Content-Type: {request.Content.Headers.ContentType}");
                Console.WriteLine($"Request Body: {JsonSerializer.Serialize(requestBody)}");

                var response = await _httpClient.SendAsync(request);
                stopwatch.Stop();

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    return (null, text.Length, 0, (int)stopwatch.ElapsedMilliseconds, $"API error: {errorContent}");
                }

                var responseContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Response Content: {responseContent}");

                var result = JsonSerializer.Deserialize<List<Dictionary<string, string>>>(responseContent);
                var summary = result?.FirstOrDefault()?["summary_text"];

                if (string.IsNullOrEmpty(summary))
                {
                    return (null, text.Length, 0, (int)stopwatch.ElapsedMilliseconds, "Empty summary returned.");
                }

                return (summary, text.Length, summary.Length, (int)stopwatch.ElapsedMilliseconds, null);
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                return (null, text.Length, 0, (int)stopwatch.ElapsedMilliseconds, ex.Message);
            }
        }

        public async Task<(string Summary, int InputLength, int OutputLength, int ExecutionTimeMs, string ErrorMessage)> SummarizeAsync_demo(SummarizeRequest summarize_request)
        {
            var apiKey = _configuration["HuggingFace:ApiKey"];
            var apiUrl = _configuration["HuggingFace:ApiUrl"];
            var stopwatch = Stopwatch.StartNew();
            var text = summarize_request.Text;
            try
            {
                if (string.IsNullOrEmpty(text))
                {
                    _logger.LogWarning("Input text is empty.");
                    return (null, 0, 0, (int)stopwatch.ElapsedMilliseconds, "Input text is empty.");
                }

                // Giới hạn độ dài input
                if (text.Length > 1000)
                {
                    text = text.Substring(0, 1000);
                    _logger.LogInformation("Input text truncated to 1000 characters.");
                }

                var requestBody = new
                {
                    inputs = text,
                    parameters = new
                    {
                        max_length = 200,
                        min_length = 50,
                        do_sample = false
                    }
                };

                var request = new HttpRequestMessage(HttpMethod.Post, apiUrl)
                {
                    Content = new StringContent(JsonSerializer.Serialize(requestBody))
                };

                // Đặt Content-Type là application/json
                request.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");

                // Đặt Authorization
                request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", apiKey);

                // Log yêu cầu
                _logger.LogInformation($"Sending request to Hugging Face API. Content-Type: {request.Content.Headers.ContentType}");
                _logger.LogDebug($"Request Body: {JsonSerializer.Serialize(requestBody)}");

                var response = await _httpClient.SendAsync(request);
                stopwatch.Stop();

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError($"Hugging Face API error: {errorContent}");
                    return (null, text.Length, 0, (int)stopwatch.ElapsedMilliseconds, $"API error: {errorContent}");
                }

                var responseContent = await response.Content.ReadAsStringAsync();
                _logger.LogDebug($"Response Content: {responseContent}");

                var result = JsonSerializer.Deserialize<List<Dictionary<string, string>>>(responseContent);
                var summary = result?.FirstOrDefault()?["summary_text"];

                if (string.IsNullOrEmpty(summary))
                {
                    _logger.LogWarning("Empty summary returned from Hugging Face API.");
                    return (null, text.Length, 0, (int)stopwatch.ElapsedMilliseconds, "Empty summary returned.");
                }

                _logger.LogInformation($"Summarization successful. InputLength: {text.Length}, OutputLength: {summary.Length}, ExecutionTimeMs: {stopwatch.ElapsedMilliseconds}");
                return (summary, text.Length, summary.Length, (int)stopwatch.ElapsedMilliseconds, null);
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                _logger.LogError($"Unexpected error in HuggingFaceService: {ex.Message}");
                return (null, text.Length, 0, (int)stopwatch.ElapsedMilliseconds, ex.Message);
            }
        }
    }
}
