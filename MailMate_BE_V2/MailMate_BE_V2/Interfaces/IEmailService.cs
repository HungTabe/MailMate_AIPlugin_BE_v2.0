using MailMate_BE_V2.DTOs;

namespace MailMate_BE_V2.Interfaces
{
    public interface IEmailService
    {
        Task<List<EmailDto>> GetInboxEmailsAsync(Guid userId);
        Task<EmailDto> GetEmailByIdAsync(string emailId, string userId);
        Task<List<EmailDto>> GetTop10InboxEmailsAsync(string userId);
        Task AddTagToEmailAsync(Guid userId, string emailId, string tagName);

    }
}
