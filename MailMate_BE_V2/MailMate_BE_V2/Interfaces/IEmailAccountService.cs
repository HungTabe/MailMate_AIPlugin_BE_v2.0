namespace MailMate_BE_V2.Services
{
    using MailMate_BE_V2.DTOs;
    using System;
    using System.Collections.Generic;

    public interface IEmailAccountService
    {
        Task<EmailAccountConnectGoogleResponse> ConnectGoogleEmailAccountAsync(string authorizationCode);
        Task<List<EmailAccountListResponse>> GetEmailAccountsAsync();
        Task<EmailAccountDetailResponse> GetEmailAccountByIdAsync(Guid emailAccountId);
        Task DeleteEmailAccountAsync(Guid emailAccountId);
    }
}
