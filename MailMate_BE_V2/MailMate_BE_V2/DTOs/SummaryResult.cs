namespace MailMate_BE_V2.DTOs
{
    public class SummaryResult
    {
        public string Summary { get; set; }
        public int InputLength { get; set; }
        public int OutputLength { get; set; }
        public long ExecutionTimeMs { get; set; }
        public string ErrorMessage { get; set; }
    }
}
