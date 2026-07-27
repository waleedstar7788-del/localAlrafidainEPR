using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RetailApp.Models
{
    public class SalesItem
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        // Foreign Keys
        public int SalesInvoiceId { get; set; }
        [ForeignKey(nameof(SalesInvoiceId))]
        public SalesInvoice SalesInvoice { get; set; } = null!;

        public int ProductId { get; set; }
        [ForeignKey(nameof(ProductId))]
        public Product Product { get; set; } = null!;

        public int Quantity { get; set; }
        
        public decimal UnitPrice { get; set; } // Selling Price at time of sale
        public decimal UnitCost { get; set; }  // Cost at time of sale (Snapshot)
        
        public decimal Discount { get; set; }
        public decimal SubTotal { get; set; }  // (Quantity * UnitPrice) - Discount

        public decimal ItemProfit => SubTotal - (Quantity * UnitCost);
    }
}
