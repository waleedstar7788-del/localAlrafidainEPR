using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RetailApp.Models
{
    public class PayrollRecord
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string PayrollNumber { get; set; } = string.Empty;

        public int EmployeeId { get; set; }
        [ForeignKey(nameof(EmployeeId))]
        public Employee Employee { get; set; } = null!;

        public int Month { get; set; }
        public int Year { get; set; }

        // Calculations
        public decimal BaseSalary { get; set; }
        public decimal TotalAllowances { get; set; }
        public decimal TotalBonuses { get; set; }
        public decimal TotalOvertime { get; set; }
        
        // Deductions
        public decimal PenaltyDeductions { get; set; }
        public decimal AdvanceDeduction { get; set; }
        public decimal LoanDeduction { get; set; }

        public decimal NetSalary { get; set; }

        public PayrollStatus Status { get; set; } = PayrollStatus.Draft;

        public DateTime? PaymentDate { get; set; }
        public FinancialPaymentMethod? PaymentMethod { get; set; }

        [MaxLength(100)]
        public string ReferenceNumber { get; set; } = string.Empty;
    }
}
