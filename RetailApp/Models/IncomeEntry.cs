using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RetailApp.Models
{
    public class IncomeEntry
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string IncomeNumber { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string Title { get; set; } = string.Empty;

        [MaxLength(500)]
        public string Description { get; set; } = string.Empty;

        public decimal Amount { get; set; }

        public FinancialPaymentMethod PaymentMethod { get; set; } = FinancialPaymentMethod.Cash;

        public DateTime IncomeDate { get; set; } = DateTime.Now;

        [MaxLength(100)]
        public string ReferenceNumber { get; set; } = string.Empty;

        [MaxLength(100)]
        public string CreatedBy { get; set; } = string.Empty;

        [MaxLength(100)]
        public string ApprovedBy { get; set; } = string.Empty;

        public FinancialTransactionStatus Status { get; set; } = FinancialTransactionStatus.Approved;

        // Links to Chart of Accounts (e.g. 4300 Other Income)
        public int AccountId { get; set; }
        [ForeignKey(nameof(AccountId))]
        public Account Account { get; set; } = null!;
    }
}
