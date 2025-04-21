using MailMate_BE_V2.DTO;

namespace MailMate_BE_V2.Interfaces
{
    public interface IAuthService
    {
        Task<LoginResponse> LoginAsync(string email, string password);
    }
}
