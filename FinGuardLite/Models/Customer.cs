using System.ComponentModel.DataAnnotations;

namespace FinGuardLite.Models
{
    public class Customer
    {
        [Key]
        public int CustomerId { get; set; }

        [Required]
        [StringLength(100)]
        public string FullName { get; set; } = string.Empty;

        [Range(16, 100)]
        public int Age { get; set; }

        [Range(0, 1000000)]
        public decimal MonthlyIncome { get; set; }

        [Required]
        [StringLength(50)]
        public string Country { get; set; } = "Singapore";

        [Required]
        [StringLength(20)]
        public string RiskProfile { get; set; } = "Low";

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public List<TransactionRecord> Transactions { get; set; } = new();
    }
}