using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FinGuardLite.Models
{
    public class TransactionRecord
    {
        [Key]
        public int TransactionRecordId { get; set; }

        [Required]
        public int CustomerId { get; set; }

        public Customer? Customer { get; set; }

        [Required]
        public DateTime TransactionDate { get; set; } = DateTime.Now;

        [Range(0.01, 1000000)]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }

        [Required]
        [StringLength(100)]
        public string Merchant { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string Category { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string Country { get; set; } = "Singapore";

        [Required]
        [StringLength(30)]
        public string PaymentType { get; set; } = "Card";

        public int RiskScore { get; set; } = 0;

        [StringLength(20)]
        public string RiskLevel { get; set; } = "Low";

        [StringLength(500)]
        public string FlagReason { get; set; } = "No major risk detected.";

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}