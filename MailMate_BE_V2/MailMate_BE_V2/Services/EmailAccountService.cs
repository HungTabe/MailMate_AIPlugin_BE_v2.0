using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Auth.OAuth2.Responses;
using MailMate_BE_V2.Data;
using MailMate_BE_V2.DTOs;
using MailMate_BE_V2.Models;
using Microsoft.EntityFrameworkCore;

namespace MailMate_BE_V2.Services
{
    public class EmailAccountService : IEmailAccountService
    {
        private readonly MailMateDbContext _context;
        private readonly string _googleClientId;
        private readonly string _googleClientSecret;
        private readonly string _redirectUri;

        public EmailAccountService(MailMateDbContext context, IConfiguration configuration)
        {
            _context = context;
            _googleClientId = configuration["GoogleOAuth:ClientId"];
            _googleClientSecret = configuration["GoogleOAuth:ClientSecret"];
            _redirectUri = configuration["GoogleOAuth:RedirectUri"];
        }

        public async Task<EmailAccountConnectGoogleResponse> ConnectGoogleEmailAccountAsync(string authorizationCode)
        {
            // Tạo đối tượng GoogleAuthorizationCodeFlow để xử lý OAuth
            var flow = new GoogleAuthorizationCodeFlow(new GoogleAuthorizationCodeFlow.Initializer
            {
                ClientSecrets = new ClientSecrets
                {
                    ClientId = _googleClientId,
                    ClientSecret = _googleClientSecret
                },
                Scopes = new[] { "https://www.googleapis.com/auth/gmail.modify" }
            });

            // Đổi mã ủy quyền lấy access token và refresh token
            var tokenResponse = await flow.ExchangeCodeForTokenAsync(
                "user",
                authorizationCode,
                _redirectUri,
                CancellationToken.None);

            // Tạo tài khoản email mới với provider Google
            var userId = Guid.NewGuid(); // Thay bằng logic lấy user ID thực tế
            var emailAccount = new EmailAccount
            {
                EmailAccountId = Guid.NewGuid(),
                UserId = userId,
                Provider = "Google",
                AccessToken = tokenResponse.AccessToken,
                RefreshToken = tokenResponse.RefreshToken,
                ConnectedAt = DateTime.UtcNow
            };

            _context.EmailAccounts.Add(emailAccount);
            await _context.SaveChangesAsync();

            return new EmailAccountConnectGoogleResponse
            {
                EmailAccountId = emailAccount.EmailAccountId,
                Provider = emailAccount.Provider,
                ConnectedAt = emailAccount.ConnectedAt
            };
        }
    }
}