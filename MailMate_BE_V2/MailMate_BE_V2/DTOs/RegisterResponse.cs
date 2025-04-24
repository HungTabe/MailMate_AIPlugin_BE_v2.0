namespace MailMate_BE_V2.DTOs
{
    public class RegisterResponse
    {
        public Guid UserId { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
        public string Message { get; set; }
    }
}
