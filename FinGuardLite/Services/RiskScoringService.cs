using FinGuardLite.Models;

namespace FinGuardLite.Services
{
    public class RiskScoringResult
    {
        public int RiskScore { get; set; }
        public string RiskLevel { get; set; } = "Low";
        public string FlagReason { get; set; } = "No major risk detected.";
    }

    public class RiskScoringService
    {
        public RiskScoringResult CalculateRisk(
            TransactionRecord transaction,
            Customer? customer,
            int transactionsTodayCount)
        {
            int score = 0;
            List<string> reasons = new();

            // Rule 1: High transaction amount
            if (transaction.Amount >= 10000)
            {
                score += 50;
                reasons.Add("Very high transaction amount");
            }
            else if (transaction.Amount >= 5000)
            {
                score += 30;
                reasons.Add("High transaction amount");
            }

            // Rule 2: Overseas transaction
            if (!transaction.Country.Equals("Singapore", StringComparison.OrdinalIgnoreCase))
            {
                score += 15;
                reasons.Add("Overseas transaction");
            }

            // Rule 3: Unknown merchant
            if (transaction.Merchant.Equals("Unknown", StringComparison.OrdinalIgnoreCase))
            {
                score += 15;
                reasons.Add("Unknown merchant");
            }

            // Rule 4: High-risk transaction category
            string[] highRiskCategories = { "Crypto", "Gambling", "Unknown", "Money Transfer" };

            if (highRiskCategories.Contains(transaction.Category))
            {
                score += 25;
                reasons.Add("High-risk transaction category");
            }

            // Rule 5: Spending does not match income profile
            if (customer != null && customer.MonthlyIncome > 0)
            {
                decimal incomeThreshold = customer.MonthlyIncome * 0.5m;

                if (transaction.Amount > incomeThreshold)
                {
                    score += 25;
                    reasons.Add("Transaction amount exceeds 50% of customer's monthly income");
                }
            }

            // Rule 6: New account with large transaction
            if (customer != null)
            {
                int accountAgeDays = (DateTime.Now - customer.CreatedAt).Days;

                if (accountAgeDays < 7 && transaction.Amount > 1000)
                {
                    score += 20;
                    reasons.Add("Large transaction from newly created account");
                }
            }

            // Rule 7: Repeated transactions in one day
            if (transactionsTodayCount >= 3)
            {
                score += 20;
                reasons.Add("Multiple transactions made by same customer in one day");
            }

            string riskLevel;

            if (score >= 60)
            {
                riskLevel = "High";
            }
            else if (score >= 30)
            {
                riskLevel = "Medium";
            }
            else
            {
                riskLevel = "Low";
            }

            return new RiskScoringResult
            {
                RiskScore = score,
                RiskLevel = riskLevel,
                FlagReason = reasons.Any()
                    ? string.Join("; ", reasons)
                    : "No major risk detected."
            };
        }
    }
}