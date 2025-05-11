namespace MailMate_BE_V2.DTOs
{
    public class SummarizeResponse
    {
        public string Summary { get; set; }
        public int InputLength { get; set; }
        public int OutputLength { get; set; }
        public int ExecutionTimeMs { get; set; }
        public string ErrorMessage { get; set; }
    }
}
