using MailMate_BE_V2.DTOs;
using System.Threading.Tasks;

namespace MailMate_BE_V2.Services
{
    public interface IAuthService
    {
        Task<RegisterResponse> RegisterAsync(RegisterRequest request);
    }
}
