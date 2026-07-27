using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RetailApp.Models
{
    public class EmployeeAdvance
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int EmployeeId { get; set; }
        [ForeignKey(nameof(EmployeeId))]
        public Employee Employee { get; set; } = null!;

        [Required]
        [MaxLength(50)]
        public string AdvanceNumber { get; set; } = string.Empty;

        public DateTime AdvanceDate { get; set; } = DateTime.Now;

        public decimal TotalAmount { get; set; }
        public decimal RemainingBalance { get; set; }

        [MaxLength(500)]
        public string Notes { get; set; } = string.Empty;

        public AdvanceStatus Status { get; set; } = AdvanceStatus.Active;
    }
}
