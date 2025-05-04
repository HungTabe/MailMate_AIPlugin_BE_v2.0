namespace MailMate_BE_V2.Services
{
    using MailMate_BE_V2.DTOs;

    public interface IEmailAccountService
    {
        Task<EmailAccountConnectGoogleResponse> ConnectGoogleEmailAccountAsync(string authorizationCode);
    }
}