using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RetailApp.Models
{
    public class InstallmentContract
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string ContractNumber { get; set; } = string.Empty;

        public int CustomerId { get; set; }
        [ForeignKey(nameof(CustomerId))]
        public Customer Customer { get; set; } = null!;

        // Optional link to a sales invoice
        public int? SalesInvoiceId { get; set; }
        [ForeignKey(nameof(SalesInvoiceId))]
        public SalesInvoice? SalesInvoice { get; set; }

        public DateTime ContractDate { get; set; } = DateTime.Now;
        public DateTime StartDate { get; set; } = DateTime.Now;
        public DateTime EndDate { get; set; } = DateTime.Now.AddMonths(1);

        public InstallmentType InstallmentType { get; set; } = InstallmentType.Monthly;

        public decimal TotalAmount { get; set; }
        public decimal DownPayment { get; set; }
        public decimal RemainingAmount { get; set; }

        public int NumberOfInstallments { get; set; }
        public decimal InstallmentAmount { get; set; }
        
        public decimal Interest { get; set; }
        public decimal Discount { get; set; }

        [MaxLength(1000)]
        public string Notes { get; set; } = string.Empty;

        public ContractStatus Status { get; set; } = ContractStatus.Active;

        public DateTime CreatedDate { get; set; } = DateTime.Now;

        public ICollection<InstallmentSchedule> Schedules { get; set; } = new List<InstallmentSchedule>();
    }
}
