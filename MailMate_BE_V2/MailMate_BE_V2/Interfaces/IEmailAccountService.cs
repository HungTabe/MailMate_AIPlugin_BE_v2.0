namespace MailMate_BE_V2.Interfaces
{
    using MailMate_BE_V2.DTOs;
    using System;
    using System.Collections.Generic;

    public interface IEmailAccountService
    {
        Task<AuthUrlResponse> GetAuthUrlAsync();
        Task<string> HandleOAuthCallbackAsync(string code);
        Task<List<EmailAccountListResponse>> GetEmailAccountsAsync();
        Task<EmailAccountDetailResponse> GetEmailAccountByIdAsync(Guid emailAccountId);
        Task DeleteEmailAccountAsync(Guid emailAccountId);
    }
}