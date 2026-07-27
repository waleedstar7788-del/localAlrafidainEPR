using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RetailApp.Models
{
    public class StockMovement
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        
        public int ProductId { get; set; }
        
        [ForeignKey(nameof(ProductId))]
        public Product Product { get; set; } = null!;
        
        public DateTime Date { get; set; } = DateTime.Now;
        
        [MaxLength(20)]
        public string Time { get; set; } = string.Empty;
        
        [MaxLength(100)]
        public string User { get; set; } = string.Empty;
        
        [MaxLength(50)]
        public string Type { get; set; } = string.Empty; // Sale, Purchase, Adjustment, Return, Manual Correction, Damaged, Expired
        
        [MaxLength(500)]
        public string Reason { get; set; } = string.Empty;
        
        [MaxLength(100)]
        public string ReferenceNumber { get; set; } = string.Empty;
        public int QuantityChange { get; set; }
        public int ResultingQuantity { get; set; }
    }
}
