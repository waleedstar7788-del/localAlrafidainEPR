using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RetailApp.Models
{
    public enum InvoiceStatus
    {
        Draft,
        Pending,
        Completed,
        Cancelled
    }

    public enum PaymentMethod
    {
        Cash,
        BankTransfer,
        Credit,
        Installment,
        Mixed
    }

    public class PurchaseInvoice
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string InvoiceNumber { get; set; } = string.Empty;

        public DateTime InvoiceDate { get; set; } = DateTime.Now;

        // Foreign Keys
        public int SupplierId { get; set; }
        [ForeignKey(nameof(SupplierId))]
        public Supplier Supplier { get; set; } = null!;

        [MaxLength(100)]
        public string WarehouseName { get; set; } = string.Empty;

        [MaxLength(100)]
        public string EmployeeName { get; set; } = string.Empty;

        public PaymentMethod PaymentMethod { get; set; } = PaymentMethod.Cash;

        [MaxLength(100)]
        public string ReferenceNumber { get; set; } = string.Empty;

        [MaxLength(1000)]
        public string Notes { get; set; } = string.Empty;

        public InvoiceStatus Status { get; set; } = InvoiceStatus.Draft;

        // Totals
        public decimal SubTotal { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal PaidAmount { get; set; }
        public decimal RemainingAmount => TotalAmount - PaidAmount;

        // Meta
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public DateTime? ModifiedDate { get; set; }

        // Navigation property
        public ICollection<PurchaseItem> Items { get; set; } = new List<PurchaseItem>();
    }
}
