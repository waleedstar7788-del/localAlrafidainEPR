using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RetailApp.Models
{
    public class PurchaseReturnItem
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int PurchaseReturnInvoiceId { get; set; }
        [ForeignKey(nameof(PurchaseReturnInvoiceId))]
        public PurchaseReturnInvoice PurchaseReturnInvoice { get; set; } = null!;

        public int ProductId { get; set; }
        [ForeignKey(nameof(ProductId))]
        public Product Product { get; set; } = null!;

        public int QuantityReturned { get; set; }
        
        public decimal UnitPrice { get; set; } // Refunded amount from supplier
        public decimal UnitCost { get; set; }  // Cost when originally purchased

        public ReturnReason Reason { get; set; } = ReturnReason.Damaged;

        public decimal SubTotal => QuantityReturned * UnitPrice;
    }
}
