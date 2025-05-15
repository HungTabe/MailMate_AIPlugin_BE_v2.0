namespace MailMate_BE_V2.DTOs
{
    public class EmailAccountDetailResponse
    {
        public Guid EmailAccountId { get; set; }
        public Guid UserId { get; set; }
        public string Provider { get; set; }
        public DateTime ConnectedAt { get; set; }
        //public bool IsActive { get; set; }
    }
}