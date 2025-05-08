using System.ComponentModel.DataAnnotations.Schema;

namespace MailMate_BE_V2.Models
{
    public class EmailTagMapping
    {
        public string EmailId { get; set; }

        public Guid EmailTagId { get; set; }

        // Navigation properties
        [ForeignKey("EmailId")]
        public virtual Email Email { get; set; }

        [ForeignKey("EmailTagId")]
        public virtual EmailTag EmailTag { get; set; }
    }
}
