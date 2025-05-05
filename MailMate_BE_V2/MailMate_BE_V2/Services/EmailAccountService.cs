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
            var userId = Guid.NewGuid(); // chưa implement logic lấy userId thực tế (vẫn hardcode userId bằng Guid.NewGuid()), API này sẽ tạm thời lấy tất cả tài khoản email trong cơ sở dữ liệu (không lọc theo userId). Sau này, khi bạn thêm logic lấy userId, chúng ta sẽ cập nhật lại để lọc theo người dùng.
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
        public async Task<List<EmailAccountListResponse>> GetEmailAccountsAsync()
        {
            // Tạm thời lấy tất cả tài khoản email vì chưa có logic lấy userId
            var emailAccounts = await _context.EmailAccounts
                .Select(ea => new EmailAccountListResponse
                {
                    EmailAccountId = ea.EmailAccountId,
                    Provider = ea.Provider,
                    ConnectedAt = ea.ConnectedAt
                })
                .ToListAsync();

            return emailAccounts;
        }
        public async Task<EmailAccountDetailResponse> GetEmailAccountByIdAsync(Guid emailAccountId)
        {
            var emailAccount = await _context.EmailAccounts
                .Where(ea => ea.EmailAccountId == emailAccountId)
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
        public async Task DeleteEmailAccountAsync(Guid emailAccountId)
        {
            var emailAccount = await _context.EmailAccounts.FindAsync(emailAccountId);
            if (emailAccount == null)
            {
                throw new KeyNotFoundException($"Không tìm thấy tài khoản email với ID {emailAccountId}.");
            }
            _context.EmailAccounts.Remove(emailAccount);
            await _context.SaveChangesAsync();
        }
    }
}
