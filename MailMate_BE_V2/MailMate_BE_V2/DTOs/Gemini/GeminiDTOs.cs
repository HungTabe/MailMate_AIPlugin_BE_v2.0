namespace MailMate_BE_V2.DTOs.Gemini
{
    public class GeminiDTOs
    {
        public string Text { get; set; }

    }

    public class GeminiSettings
    {
        public string ApiKey { get; set; }
        public string Endpoint { get; set; }
    }

    public class GeminiSummarizeRequest
    {
        public string Text { get; set; }
        public int MaxLength { get; set; } = 100; // Độ dài tối đa của tóm tắt (tùy chọn)
    }

    public class SummarizeResponse
    {
        public string Summary { get; set; }
    }

    // Lớp ánh xạ phản hồi từ Gemini API
    public class GeminiResponse
    {
        public Candidate[] Candidates { get; set; }
    }

    public class Candidate
    {
        public Content Content { get; set; }
    }

    public class Content
    {
        public Part[] Parts { get; set; }
    }

    public class Part
    {
        public string Text { get; set; }
    }
}
