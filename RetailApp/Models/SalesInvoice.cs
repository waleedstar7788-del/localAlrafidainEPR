using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RetailApp.Models
{
    public enum SalesInvoiceStatus
    {
        Paid,
        Partial,
        Unpaid,
        Credit,
        Cancelled
    }

    public class SalesInvoice
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string InvoiceNumber { get; set; } = string.Empty;

        public DateTime InvoiceDate { get; set; } = DateTime.Now;

        // Foreign Keys
        public int? CustomerId { get; set; }
        [ForeignKey(nameof(CustomerId))]
        public Customer? Customer { get; set; }

        [MaxLength(100)]
        public string CashierName { get; set; } = string.Empty;

        [MaxLength(50)]
        public string PaymentMethod { get; set; } = "Cash";

        [MaxLength(1000)]
        public string Notes { get; set; } = string.Empty;

        public SalesInvoiceStatus Status { get; set; } = SalesInvoiceStatus.Paid;

        // Totals
        public decimal SubTotal { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal GrandTotal { get; set; }
        
        public decimal PaidAmount { get; set; }
        public decimal RemainingAmount { get; set; }

        // Profit Engine Snapshot
        public decimal TotalCost { get; set; }
        public decimal GrossProfit { get; set; }
        public decimal NetProfit { get; set; }

        // Meta
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public DateTime? ModifiedDate { get; set; }

        // Navigation property
        public ICollection<SalesItem> Items { get; set; } = new List<SalesItem>();
    }
}
