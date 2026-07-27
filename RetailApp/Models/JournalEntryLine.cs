using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RetailApp.Models
{
    public class JournalEntryLine
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int JournalEntryId { get; set; }
        [ForeignKey(nameof(JournalEntryId))]
        public JournalEntry JournalEntry { get; set; } = null!;

        public int AccountId { get; set; }
        [ForeignKey(nameof(AccountId))]
        public Account Account { get; set; } = null!;

        public decimal DebitAmount { get; set; }
        public decimal CreditAmount { get; set; }

        [MaxLength(500)]
        public string Description { get; set; } = string.Empty;
    }
}
