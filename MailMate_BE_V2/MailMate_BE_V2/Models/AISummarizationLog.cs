using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace MailMate_BE_V2.Models
{
    public class AISummarizationLog
    {
        [Key]
        public Guid LogId { get; set; }

        [Required]
        [StringLength(450)]
        public string EmailId { get; set; }

        [StringLength(100)]
        public string ModelName { get; set; }

        public int InputLength { get; set; }

        public int OutputLength { get; set; }

        public int ExecutionTimeMs { get; set; }

        [StringLength(50)]
        public string Status { get; set; }

        public string ErrorMessage { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; }

        [ForeignKey("EmailId")]
        public virtual Email Email { get; set; }
    }
}
