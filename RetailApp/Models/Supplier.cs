using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RetailApp.Models
{
    public enum SupplierType
    {
        LocalSupplier,
        InternationalSupplier,
        Manufacturer,
        Distributor,
        Importer,
        Wholesaler,
        Custom
    }

    public class Supplier
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string SupplierNumber { get; set; } = string.Empty; // e.g. "SUP-10001"

        [Required]
        [MaxLength(150)]
        public string SupplierName { get; set; } = string.Empty;

        [MaxLength(150)]
        public string CompanyName { get; set; } = string.Empty;

        public SupplierType Type { get; set; } = SupplierType.LocalSupplier;

        // Contact
        [MaxLength(20)]
        public string Phone1 { get; set; } = string.Empty;
        
        [MaxLength(20)]
        public string Phone2 { get; set; } = string.Empty;
        
        [MaxLength(20)]
        public string WhatsApp { get; set; } = string.Empty;
        
        [MaxLength(100)]
        public string Email { get; set; } = string.Empty;
        
        [MaxLength(100)]
        public string Country { get; set; } = string.Empty;
        
        [MaxLength(100)]
        public string City { get; set; } = string.Empty;
        
        [MaxLength(500)]
        public string Address { get; set; } = string.Empty;
        
        [MaxLength(500)]
        public string GoogleMapsLink { get; set; } = string.Empty;
        
        [MaxLength(500)]
        public string Website { get; set; } = string.Empty;

        // Legal & Banking
        [MaxLength(50)]
        public string TaxNumber { get; set; } = string.Empty;
        
        [MaxLength(50)]
        public string CommercialRegistration { get; set; } = string.Empty;
        
        [MaxLength(100)]
        public string BankName { get; set; } = string.Empty;
        
        [MaxLength(100)]
        public string BankAccount { get; set; } = string.Empty;
        
        [MaxLength(100)]
        public string IBAN { get; set; } = string.Empty;

        [MaxLength(1000)]
        public string Notes { get; set; } = string.Empty;
        
        public string SupplierPhotoPath { get; set; } = string.Empty;

        // Financials
        public decimal OpeningBalance { get; set; }
        public decimal CurrentBalance { get; set; }
        public decimal TotalPurchases { get; set; }
        public decimal TotalPayments { get; set; }
        public decimal OutstandingAmount => CurrentBalance; // Shortcut
        public double SupplierRating { get; set; } = 5.0;

        // Meta
        public bool IsActive { get; set; } = true;
        public DateTime RegistrationDate { get; set; } = DateTime.Now;
        public DateTime? LastPurchaseDate { get; set; }
        public DateTime? LastPaymentDate { get; set; }
        
        [MaxLength(50)]
        public string CreatedBy { get; set; } = string.Empty;
        
        [MaxLength(50)]
        public string ModifiedBy { get; set; } = string.Empty;
        
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public DateTime? ModifiedDate { get; set; }
    }
}
