using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RetailApp.Models
{
    public enum CustomerType
    {
        Individual,
        Company,
        Government
    }

    public enum CustomerRank
    {
        Retail,
        Wholesale,
        VIP,
        Distributor,
        Contractor,
        Government,
        Custom
    }

    public class Customer
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string CustomerNumber { get; set; } = string.Empty;

        [Required]
        [MaxLength(150)]
        public string FullName { get; set; } = string.Empty;

        [MaxLength(150)]
        public string CompanyName { get; set; } = string.Empty;

        public CustomerType Type { get; set; } = CustomerType.Individual;
        public CustomerRank Rank { get; set; } = CustomerRank.Retail;

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

        [MaxLength(50)]
        public string TaxNumber { get; set; } = string.Empty;
        
        [MaxLength(50)]
        public string NationalId { get; set; } = string.Empty;
        
        [MaxLength(50)]
        public string CommercialRegistration { get; set; } = string.Empty;

        [MaxLength(1000)]
        public string Notes { get; set; } = string.Empty;
        
        public string ProfilePhotoPath { get; set; } = string.Empty;

        public decimal OpeningBalance { get; set; }
        public decimal CurrentBalance { get; set; }
        public decimal CreditLimit { get; set; }
        public decimal TotalSales { get; set; }
        public decimal TotalPurchases { get; set; }
        public decimal TotalPaid { get; set; }
        public decimal TotalDue => CurrentBalance;

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
