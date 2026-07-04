using System.ComponentModel.DataAnnotations;

namespace FinGuardLite.Models
{
    public class RiskAlert
    {
        [Key]
        public int RiskAlertId { get; set; }

        [Required]
        public int TransactionRecordId { get; set; }

        public TransactionRecord? TransactionRecord { get; set; }

        public int RiskScore { get; set; }

        [StringLength(20)]
        public string RiskLevel { get; set; } = "Medium";

        [StringLength(500)]
        public string Reason { get; set; } = string.Empty;

        [StringLength(30)]
        public string Status { get; set; } = "Open";

        [StringLength(1000)]
        public string AnalystComment { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public DateTime? UpdatedAt { get; set; }
    }
}