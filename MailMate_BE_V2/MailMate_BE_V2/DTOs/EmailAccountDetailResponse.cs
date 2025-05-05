namespace MailMate_BE_V2.DTOs
{
    public class EmailAccountDetailResponse
    {
        public Guid EmailAccountId { get; set; } // ID của tài khoản email
        public Guid UserId { get; set; } // ID của người dùng sở hữu
        public string Provider { get; set; } // Nhà cung cấp (Google)
        public DateTime ConnectedAt { get; set; } // Thời gian kết nối
    }
}