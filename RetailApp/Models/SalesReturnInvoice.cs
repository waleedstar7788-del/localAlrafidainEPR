using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RetailApp.Models
{
    public class SalesReturnInvoice
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string ReturnNumber { get; set; } = string.Empty;

        public DateTime ReturnDate { get; set; } = DateTime.Now;

        // Optional link to original sale
        public int? SalesInvoiceId { get; set; }
        [ForeignKey(nameof(SalesInvoiceId))]
        public SalesInvoice? OriginalSalesInvoice { get; set; }

        public int? CustomerId { get; set; }
        [ForeignKey(nameof(CustomerId))]
        public Customer? Customer { get; set; }

        [MaxLength(100)]
        public string CashierName { get; set; } = string.Empty;

        public RefundMethod RefundMethod { get; set; } = RefundMethod.CashRefund;
        public ReturnStatus Status { get; set; } = ReturnStatus.Completed;

        [MaxLength(1000)]
        public string Notes { get; set; } = string.Empty;

        public decimal TotalRefundAmount { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.Now;

        public ICollection<SalesReturnItem> Items { get; set; } = new List<SalesReturnItem>();
    }
}
