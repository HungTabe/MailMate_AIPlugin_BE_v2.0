using MailMate_BE_V2.DTOs;

namespace MailMate_BE_V2.Interfaces
{
    public interface IGeminiService
    {
        Task<SummaryResult> GeminiSummarizeTextAsync(string text, int maxLength);

    }
}
