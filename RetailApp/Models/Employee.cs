using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RetailApp.Models
{
    public class Employee
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string EmployeeNumber { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string FullName { get; set; } = string.Empty;

        [MaxLength(50)]
        public string NationalId { get; set; } = string.Empty;

        [MaxLength(50)]
        public string Phone { get; set; } = string.Empty;

        [MaxLength(100)]
        public string Email { get; set; } = string.Empty;

        [MaxLength(200)]
        public string Address { get; set; } = string.Empty;

        [MaxLength(100)]
        public string Department { get; set; } = string.Empty;

        [MaxLength(100)]
        public string JobTitle { get; set; } = string.Empty;

        public DateTime HireDate { get; set; } = DateTime.Now;

        public ContractType ContractType { get; set; } = ContractType.FullTime;
        public EmploymentStatus Status { get; set; } = EmploymentStatus.Active;

        // Financials
        public decimal MonthlySalary { get; set; }
        public decimal DailySalary { get; set; }
        public decimal HourlySalary { get; set; }
        public decimal CommissionPercentage { get; set; }
        public decimal StandardAllowance { get; set; }
        public decimal OvertimeRate { get; set; }

        [MaxLength(100)]
        public string BankAccount { get; set; } = string.Empty;

        [MaxLength(1000)]
        public string Notes { get; set; } = string.Empty;
    }
}
