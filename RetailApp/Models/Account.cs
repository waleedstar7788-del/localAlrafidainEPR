using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RetailApp.Models
{
    public class Account
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [MaxLength(20)]
        public string AccountNumber { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        public AccountCategory Category { get; set; }

        public int? ParentAccountId { get; set; }
        [ForeignKey(nameof(ParentAccountId))]
        public Account? ParentAccount { get; set; }

        public bool IsActive { get; set; } = true;
        
        public bool IsSystemAccount { get; set; } = false; // Used to prevent deletion of hardcoded default accounts

        public DateTime CreatedDate { get; set; } = DateTime.Now;
    }
}
