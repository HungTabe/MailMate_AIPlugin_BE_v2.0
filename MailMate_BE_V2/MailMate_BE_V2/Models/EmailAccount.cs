using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace MailMate_BE_V2.Models
{
    public class EmailAccount
    {
        [Key]
        public Guid EmailAccountId { get; set; }

        [Required]
        public Guid UserId { get; set; }

        [Required]
        [StringLength(50)]
        public string Provider { get; set; }
        [Required]
        [StringLength(255)] // Địa chỉ email tối đa 255 ký tự
        [EmailAddress] // Kiểm tra định dạng email
        public string Email { get; set; } // Thêm thuộc tính mới cho API gửi mail thủ công

        [Required]
        [StringLength(500)]
        public string AccessToken { get; set; }

        [StringLength(500)]
        public string RefreshToken { get; set; }

        [Required]
        public DateTime ConnectedAt { get; set; }

        // Navigation properties
        [ForeignKey("UserId")]
        public virtual User User { get; set; }

        public virtual ICollection<Email> Emails { get; set; }
        public virtual ICollection<EmailSchedule> EmailSchedules { get; set; }
    }
}
