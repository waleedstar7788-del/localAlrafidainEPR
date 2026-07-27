using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RetailApp.Models
{
    public class InstallmentSchedule
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int InstallmentContractId { get; set; }
        [ForeignKey(nameof(InstallmentContractId))]
        public InstallmentContract InstallmentContract { get; set; } = null!;

        public int InstallmentNumber { get; set; }

        public DateTime DueDate { get; set; }

        public decimal Amount { get; set; }
        public decimal PaidAmount { get; set; }
        
        [NotMapped]
        public decimal RemainingAmount => Amount - PaidAmount;

        public ScheduleStatus Status { get; set; } = ScheduleStatus.Pending;

        public DateTime? LastPaymentDate { get; set; }

        [MaxLength(500)]
        public string Notes { get; set; } = string.Empty;

        public ICollection<InstallmentPayment> Payments { get; set; } = new List<InstallmentPayment>();
    }
}
