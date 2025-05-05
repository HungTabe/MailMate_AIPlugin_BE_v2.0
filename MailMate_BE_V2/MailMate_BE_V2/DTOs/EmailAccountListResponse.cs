namespace MailMate_BE_V2.DTOs
{
    public class EmailAccountListResponse
    {
        public Guid EmailAccountId { get; set; } // ID của tài khoản email
        public string Provider { get; set; } // Nhà cung cấp (Google)
        public DateTime ConnectedAt { get; set; } // Thời gian kết nối
    }
}
