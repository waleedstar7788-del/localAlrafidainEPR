using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RetailApp.Models
{
    public class AuditLog
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public DateTime Timestamp { get; set; } = DateTime.Now;

        public int? UserId { get; set; }
        
        [MaxLength(100)]
        public string Username { get; set; } = string.Empty;

        [MaxLength(50)]
        public string ModuleName { get; set; } = string.Empty;

        [MaxLength(50)]
        public string Action { get; set; } = string.Empty; // e.g., "Create", "Delete", "Login"

        [MaxLength(1000)]
        public string Description { get; set; } = string.Empty;

        [MaxLength(100)]
        public string ComputerName { get; set; } = string.Empty;
    }
}
