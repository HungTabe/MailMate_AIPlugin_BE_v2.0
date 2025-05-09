using Google.Apis.Auth.OAuth2.Responses;
using MailMate_BE_V2.Data;
using MailMate_BE_V2.DTOs;
using MailMate_BE_V2.Interfaces;
using MailMate_BE_V2.Models;
using Microsoft.EntityFrameworkCore;
using System.Net.Http;
using System.Security.Claims;

namespace MailMate_BE_V2.Services
{
    public class EmailAccountService : IEmailAccountService
    {
        private readonly IConfiguration _configuration;
        private readonly HttpClient _httpClient;
        private readonly MailMateDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public EmailAccountService(IConfiguration configuration, MailMateDbContext context, HttpClient httpClient, IHttpContextAccessor httpContextAccessor)
        {
            _configuration = configuration;
            _context = context;
            _httpClient = httpClient;
            _httpContextAccessor = httpContextAccessor;
        }

        public Task<AuthUrlResponse> GetAuthUrlAsync()
        {
            var clientId = _configuration["GoogleOAuth:ClientId"];
            var redirectUri = _configuration["GoogleOAuth:RedirectUri"];
            var scopes = new[] { "https://www.googleapis.com/auth/gmail.readonly", "email", "profile" }; // Thêm scopes khác nếu cần

            if (string.IsNullOrEmpty(clientId) || string.IsNullOrEmpty(redirectUri))
            {
                throw new InvalidOperationException("Google OAuth configuration is missing.");
            }

            var authUrl = $"https://accounts.google.com/o/oauth2/auth?" +
                          $"client_id={Uri.EscapeDataString(clientId)}&" +
                          $"redirect_uri={Uri.EscapeDataString(redirectUri)}&" +
                          $"response_type=code&" +
                          $"scope={Uri.EscapeDataString(string.Join(" ", scopes))}&" +
                          $"access_type=offline&" +
                          $"prompt=consent";

            return Task.FromResult(new AuthUrlResponse { AuthUrl = authUrl });
        }

        public async Task<string> HandleOAuthCallbackAsync(string code)
        {
            if (string.IsNullOrEmpty(code))
                throw new ArgumentException("Authorization code is missing.");

            var clientId = _configuration["GoogleOAuth:ClientId"];
            var clientSecret = _configuration["GoogleOAuth:ClientSecret"];
            var redirectUri = _configuration["GoogleOAuth:RedirectUri"];
            var successOAuthUri = _configuration["GoogleOAuth:SuccessOAuthUri"];

            if (string.IsNullOrEmpty(clientId) || string.IsNullOrEmpty(clientSecret) || string.IsNullOrEmpty(redirectUri))
                throw new InvalidOperationException("Google OAuth configuration is missing.");

            try
            {
                // Đổi code lấy token
                var tokenResponse = await _httpClient.PostAsync(
                    "https://oauth2.googleapis.com/token",
                    new FormUrlEncodedContent(new Dictionary<string, string>
                    {
                        { "code", code },
                        { "client_id", clientId },
                        { "client_secret", clientSecret },
                        { "redirect_uri", redirectUri },
                        { "grant_type", "authorization_code" }
                    })
                );

                if (!tokenResponse.IsSuccessStatusCode)
                    throw new HttpRequestException($"Failed to exchange code for token: {await tokenResponse.Content.ReadAsStringAsync()}");

                var tokenData = await tokenResponse.Content.ReadFromJsonAsync<DTOs.TokenResponse>();

                // Lấy thông tin người dùng (email address)
                var userInfoResponse = await _httpClient.GetAsync(
                    $"https://www.googleapis.com/oauth2/v2/userinfo?access_token={tokenData.access_token}"
                );
                if (!userInfoResponse.IsSuccessStatusCode)
                    throw new HttpRequestException("Failed to fetch user info.");

                var userInfo = await userInfoResponse.Content.ReadFromJsonAsync<UserInfoResponse>();

                var user = await _context.Users
                    .FirstOrDefaultAsync(u => u.Email == userInfo.email);
                if (user == null)
                {
                    throw new InvalidOperationException($"No user found with email: {userInfo.email}");
                }

                // Lưu tài khoản email
                var emailAccount = new EmailAccount
                {
                    EmailAccountId = Guid.NewGuid(),
                    UserId = user.UserId, // Chuyển từ string sang Guid
                    Provider = "Google",
                    AccessToken = tokenData.access_token,
                    RefreshToken = tokenData.refresh_token,
                    ConnectedAt = DateTime.UtcNow
                };

                _context.EmailAccounts.Add(emailAccount);
                await _context.SaveChangesAsync();

                // Trả về URL chuyển hướng thành công
                return successOAuthUri;
            }
            catch (Exception ex)
            {
                // Trả về URL lỗi
                return $"https://mailmate-dashboard.onrender.com/email-connected?error={Uri.EscapeDataString(ex.Message)}";
            }
        }

        public async Task<List<EmailAccountListResponse>> GetEmailAccountsAsync() //API Lấy danh sách tài khoản email đã kết nối
        {
            var userIdClaim = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
            {
                throw new InvalidOperationException("Không thể lấy userId từ token JWT.");
            }

            var emailAccounts = await _context.EmailAccounts
                .Where(ea => ea.UserId == userId)
                .Select(ea => new EmailAccountListResponse
                {
                    EmailAccountId = ea.EmailAccountId,
                    Provider = ea.Provider,
                    ConnectedAt = ea.ConnectedAt
                })
                .ToListAsync();

            return emailAccounts;
        }

        public async Task<EmailAccountDetailResponse> GetEmailAccountByIdAsync(Guid emailAccountId) //Lấy chi tiết tài khoản email
        {
            var userIdClaim = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
            {
                throw new InvalidOperationException("Không thể lấy userId từ token JWT.");
            }

            var emailAccount = await _context.EmailAccounts
                .Where(ea => ea.EmailAccountId == emailAccountId && ea.UserId == userId)
                .Select(ea => new EmailAccountDetailResponse
                {
                    EmailAccountId = ea.EmailAccountId,
                    UserId = ea.UserId,
                    Provider = ea.Provider,
                    ConnectedAt = ea.ConnectedAt
                })
                .FirstOrDefaultAsync();

            if (emailAccount == null)
            {
                throw new KeyNotFoundException($"Không tìm thấy tài khoản email với ID {emailAccountId}.");
            }

            return emailAccount;
        }
    }
}