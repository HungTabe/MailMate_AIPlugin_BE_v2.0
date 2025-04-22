namespace MailMate_BE_V2.DTOs
{
    public class RegisterRequest
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string FullName { get; set; }

        // Không cho phép client tự chọn Role và SubscriptionPlan để tránh lạm quyền. Gán mặc định ở Service.
    }
}
