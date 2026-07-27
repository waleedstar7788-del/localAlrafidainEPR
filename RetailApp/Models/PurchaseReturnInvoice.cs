using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RetailApp.Models
{
    public class PurchaseReturnInvoice
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string ReturnNumber { get; set; } = string.Empty;

        public DateTime ReturnDate { get; set; } = DateTime.Now;

        public int? PurchaseInvoiceId { get; set; }
        [ForeignKey(nameof(PurchaseInvoiceId))]
        public PurchaseInvoice? OriginalPurchaseInvoice { get; set; }

        public int SupplierId { get; set; }
        [ForeignKey(nameof(SupplierId))]
        public Supplier Supplier { get; set; } = null!;

        [MaxLength(100)]
        public string EmployeeName { get; set; } = string.Empty;

        public RefundMethod RefundMethod { get; set; } = RefundMethod.Credit;
        public ReturnStatus Status { get; set; } = ReturnStatus.Completed;

        [MaxLength(1000)]
        public string Notes { get; set; } = string.Empty;

        public decimal TotalRefundAmount { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.Now;

        public ICollection<PurchaseReturnItem> Items { get; set; } = new List<PurchaseReturnItem>();
    }
}
