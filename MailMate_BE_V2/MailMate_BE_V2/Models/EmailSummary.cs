using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace MailMate_BE_V2.Models
{
    public class EmailSummary
    {
        [Key]
        public Guid EmailSummaryId { get; set; }

        [Required]
        [StringLength(450)]
        public string EmailId { get; set; }

        [Required]
        public string Summary { get; set; }

        [StringLength(100)]
        public string ModelName { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        [ForeignKey("EmailId")]
        public virtual Email Email { get; set; }
    }
}
