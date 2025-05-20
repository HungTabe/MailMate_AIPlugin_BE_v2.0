namespace MailMate_BE_V2.DTOs
{
    public class SendEmailRequest
    {
        public Guid EmailAccountId { get; set; } // ID của tài khoản gửi
        public string To { get; set; } // Địa chỉ người nhận
        public string Subject { get; set; } // Tiêu đề
        public string Body { get; set; } // Nội dung
    }
}