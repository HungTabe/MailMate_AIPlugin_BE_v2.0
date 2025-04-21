namespace MailMate_BE_V2.DTO
{
    public class LoginResponse
    {
        public string Token { get; set; }
        public int ExpiresIn { get; set; }
        public UserDto User { get; set; }
    }
}
