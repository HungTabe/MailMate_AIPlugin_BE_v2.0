using System.ComponentModel.DataAnnotations;

namespace MailMate_BE_V2.Models
{
    public class EmailTag
    {
        [Key]
        public Guid EmailTagId { get; set; }

        [Required]
        [StringLength(50)]
        public string TagName { get; set; }

        // Navigation properties
        public virtual ICollection<EmailTagMapping> EmailTagMappings { get; set; }
    }
}
