using System.ComponentModel.DataAnnotations;

namespace MailMate_BE_V2.DTOs
{
    public class ConsultationRequest
    {
        public string FullName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string PlanType { get; set; } // Free, Pro, Business
    }
}
