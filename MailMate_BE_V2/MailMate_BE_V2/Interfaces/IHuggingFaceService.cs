using MailMate_BE_V2.DTOs;

namespace MailMate_BE_V2.Interfaces
{
    public interface IHuggingFaceService
    {
        Task<(string Summary, int InputLength, int OutputLength, int ExecutionTimeMs, string ErrorMessage)> SummarizeAsync(string text);

        Task<(string Summary, int InputLength, int OutputLength, int ExecutionTimeMs, string ErrorMessage)> SummarizeAsync_demo(SummarizeRequest summarize_request);
    }
}
