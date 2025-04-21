using MailMate_BE_V2.DTOs;

namespace MailMate_BE_V2.Interfaces
{
    public interface IMarketingService
    {
        Task SubmitConsultationRequestAsync(ConsultationRequest request);
    }
}
