using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RetailApp.Models
{
    public class PurchaseItem
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        // Foreign Keys
        public int PurchaseInvoiceId { get; set; }
        [ForeignKey(nameof(PurchaseInvoiceId))]
        public PurchaseInvoice PurchaseInvoice { get; set; } = null!;

        public int ProductId { get; set; }
        [ForeignKey(nameof(ProductId))]
        public Product Product { get; set; } = null!;

        [MaxLength(50)]
        public string Barcode { get; set; } = string.Empty;

        [MaxLength(50)]
        public string SKU { get; set; } = string.Empty;

        [MaxLength(50)]
        public string Unit { get; set; } = string.Empty;

        public int Quantity { get; set; }
        
        public decimal PurchasePrice { get; set; }
        public decimal Discount { get; set; }
        public decimal Tax { get; set; }
        
        public decimal SubTotal { get; set; }

        // Tracking
        public DateTime? ExpirationDate { get; set; }
        [MaxLength(100)]
        public string BatchNumber { get; set; } = string.Empty;
        public DateTime? ManufacturingDate { get; set; }
    }
}
