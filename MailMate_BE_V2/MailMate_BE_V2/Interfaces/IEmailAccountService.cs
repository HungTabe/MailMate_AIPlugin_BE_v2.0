using MailMate_BE_V2.DTOs;
using System.Collections.Generic;

namespace MailMate_BE_V2.Interfaces
{

    public interface IEmailAccountService
    {
        Task<AuthUrlResponse> GetAuthUrlAsync();
        Task<string> HandleOAuthCallbackAsync(string code);
        Task<List<EmailAccountListResponse>> GetAllEmailAccountsAsync();
        Task DeleteEmailAccountAsync(Guid userId, Guid emailAccountId);

    }
}
