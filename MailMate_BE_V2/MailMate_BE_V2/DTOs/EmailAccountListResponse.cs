namespace MailMate_BE_V2.DTOs
{
    public class EmailAccountListResponse
    {
        public Guid EmailAccountId { get; set; }
        public string Provider { get; set; }
        public DateTime ConnectedAt { get; set; }
    }
}