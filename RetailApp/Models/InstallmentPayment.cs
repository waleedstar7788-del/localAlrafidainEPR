using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RetailApp.Models
{
    public class InstallmentPayment
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int InstallmentScheduleId { get; set; }
        [ForeignKey(nameof(InstallmentScheduleId))]
        public InstallmentSchedule InstallmentSchedule { get; set; } = null!;

        [Required]
        [MaxLength(50)]
        public string ReceiptNumber { get; set; } = string.Empty;

        public DateTime PaymentDate { get; set; } = DateTime.Now;

        public decimal AmountPaid { get; set; }

        public InstallmentPaymentMethod PaymentMethod { get; set; } = InstallmentPaymentMethod.Cash;

        [MaxLength(100)]
        public string CashierName { get; set; } = string.Empty;

        [MaxLength(500)]
        public string Notes { get; set; } = string.Empty;
    }
}
