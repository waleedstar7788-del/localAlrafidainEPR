using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RetailApp.Models
{
    public class Product
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        
        [Required]
        [MaxLength(150)]
        public string Name { get; set; } = string.Empty;
        [MaxLength(150)]
        public string ArabicName { get; set; } = string.Empty;
        
        [MaxLength(150)]
        public string EnglishName { get; set; } = string.Empty;
        
        [MaxLength(50)]
        public string SKU { get; set; } = string.Empty;
        
        [MaxLength(50)]
        public string Barcode { get; set; } = string.Empty;
        
        [MaxLength(500)]
        public string QRCode { get; set; } = string.Empty;
        
        [MaxLength(100)]
        public string Category { get; set; } = string.Empty;
        [MaxLength(100)]
        public string Brand { get; set; } = string.Empty;
        
        [MaxLength(50)]
        public string Unit { get; set; } = string.Empty;
        
        // Pricing
        public decimal PurchasePrice { get; set; }
        public decimal SellingPrice { get; set; }
        public decimal WholesalePrice { get; set; }
        public decimal MinimumSellingPrice { get; set; }
        public decimal CostPrice { get; set; }
        public decimal TaxRate { get; set; }
        public bool DiscountEligibility { get; set; }
        
        // Inventory
        public int CurrentQuantity { get; set; }
        public int MinimumStock { get; set; }
        public int MaximumStock { get; set; }
        public int ReorderQuantity { get; set; }
        
        // Metadata
        [MaxLength(500)]
        public string ImageUrl { get; set; } = string.Empty;
        
        [MaxLength(1000)]
        public string Description { get; set; } = string.Empty;
        
        [MaxLength(50)]
        public string Status { get; set; } = "Active"; // Active, Inactive, Archived
        public DateTime CreationDate { get; set; } = DateTime.Now;
        public DateTime LastUpdated { get; set; } = DateTime.Now;
        public bool IsFavorite { get; set; }

        // Helper property for backward compatibility with POS logic
        [NotMapped]
        public decimal Price => SellingPrice;
        
        [NotMapped]
        public int StockQuantity => CurrentQuantity;
    }
}
