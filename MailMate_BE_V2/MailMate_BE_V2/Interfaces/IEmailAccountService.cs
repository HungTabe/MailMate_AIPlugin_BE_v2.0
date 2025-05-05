using MailMate_BE_V2.DTOs;

namespace MailMate_BE_V2.Interfaces
{
    public interface IEmailAccountService
    {
        Task<AuthUrlResponse> GetAuthUrlAsync();
        Task<string> HandleOAuthCallbackAsync(string code);
    }
}
