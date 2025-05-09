namespace MailMate_BE_V2.Interfaces
{
    public interface IGeminiService
    {
        Task<string> GeminiSummarizeTextAsync(string text, int maxLength);

    }
}
