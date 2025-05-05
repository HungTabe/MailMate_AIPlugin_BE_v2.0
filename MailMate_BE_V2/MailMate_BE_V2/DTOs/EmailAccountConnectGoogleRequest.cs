namespace MailMate_BE_V2.DTOs
{
    using System.ComponentModel.DataAnnotations;

    public class EmailAccountConnectGoogleRequest
    {
        [Required(ErrorMessage = "Mã ủy quyền là bắt buộc")]
        public string AuthorizationCode { get; set; } // Mã ủy quyền từ Google OAuth
    }
}
