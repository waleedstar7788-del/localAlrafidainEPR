using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RetailApp.Models
{
    public class SalesReturnItem
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int SalesReturnInvoiceId { get; set; }
        [ForeignKey(nameof(SalesReturnInvoiceId))]
        public SalesReturnInvoice SalesReturnInvoice { get; set; } = null!;

        public int ProductId { get; set; }
        [ForeignKey(nameof(ProductId))]
        public Product Product { get; set; } = null!;

        public int QuantityReturned { get; set; }
        
        public decimal UnitPrice { get; set; } // Refunded amount per unit
        public decimal UnitCost { get; set; }  // Original cost for profit adjustment

        public ReturnReason Reason { get; set; } = ReturnReason.CustomerRequest;

        public decimal SubTotal => QuantityReturned * UnitPrice;
    }
}
