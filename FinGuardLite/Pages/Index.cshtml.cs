using FinGuardLite.Data;
using FinGuardLite.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace FinGuardLite.Pages
{
    public class IndexModel : PageModel
    {
        private readonly FinGuardDbContext _context;

        public IndexModel(FinGuardDbContext context)
        {
            _context = context;
        }

        public int TotalTransactions { get; set; }
        public decimal TotalTransactionValue { get; set; }
        public int FlaggedTransactions { get; set; }
        public int HighRiskTransactions { get; set; }
        public int OpenAlerts { get; set; }

        public IList<RiskAlert> RecentRiskAlerts { get; set; } = new List<RiskAlert>();

        public async Task OnGetAsync()
        {
            TotalTransactions = await _context.TransactionRecords.CountAsync();

            TotalTransactionValue = await _context.TransactionRecords.AnyAsync()
                ? await _context.TransactionRecords.SumAsync(t => t.Amount)
                : 0;

            FlaggedTransactions = await _context.TransactionRecords
                .CountAsync(t => t.RiskScore >= 30);

            HighRiskTransactions = await _context.TransactionRecords
                .CountAsync(t => t.RiskLevel == "High");

            OpenAlerts = await _context.RiskAlerts
                .CountAsync(a => a.Status == "Open");

            RecentRiskAlerts = await _context.RiskAlerts
                .Include(a => a.TransactionRecord!)
                .ThenInclude(t => t.Customer)
                .OrderByDescending(a => a.CreatedAt)
                .Take(5)
                .ToListAsync();
        }
    }
}