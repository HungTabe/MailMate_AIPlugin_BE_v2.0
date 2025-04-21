namespace MailMate_BE_V2.DTO
{
    public class UserDto
    {
        public Guid UserId { get; set; }
        public string Email { get; set; }
        public string FullName { get; set; }
        public string Role { get; set; }
    }
}
