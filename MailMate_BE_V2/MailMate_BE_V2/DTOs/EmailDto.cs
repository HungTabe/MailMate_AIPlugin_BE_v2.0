namespace MailMate_BE_V2.DTOs
{
    public class EmailDto
    {
        public string EmailId { get; set; }
        public string? MessageId { get; set; }
        public string? EmailDetailGGLink { get; set; }
        public string Subject { get; set; }
        public string? Summary { get; set; }
        public string? Body { get; set; }
        public bool IsSpam { get; set; }
        public DateTime ReceivedAt { get; set; }
        public string From { get; set; }
    }
}
