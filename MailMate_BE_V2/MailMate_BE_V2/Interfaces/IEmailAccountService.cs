namespace MailMate_BE_V2.Interfaces
{
    using MailMate_BE_V2.DTOs;
    using System.Collections.Generic;

    public interface IEmailAccountService
    {
        Task<AuthUrlResponse> GetAuthUrlAsync();
        Task<string> HandleOAuthCallbackAsync(string code);
        Task<List<EmailAccountListResponse>> GetEmailAccountsAsync();
    }
}