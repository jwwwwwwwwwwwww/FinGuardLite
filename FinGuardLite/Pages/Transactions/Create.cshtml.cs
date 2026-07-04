using FinGuardLite.Data;
using FinGuardLite.Models;
using FinGuardLite.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace FinGuardLite.Pages.Transactions
{
    public class CreateModel : PageModel
    {
        private readonly FinGuardDbContext _context;
        private readonly RiskScoringService _riskScoringService;

        public CreateModel(FinGuardDbContext context, RiskScoringService riskScoringService)
        {
            _context = context;
            _riskScoringService = riskScoringService;
        }

        [BindProperty]
        public TransactionRecord TransactionRecord { get; set; } = new();

        public SelectList CustomerSelectList { get; set; } = default!;

        public List<string> Countries { get; set; } = new();

        public async Task OnGetAsync()
        {
            TransactionRecord.TransactionDate = DateTime.Now;
            await PopulateCustomersAsync();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            await PopulateCustomersAsync();

            if (!ModelState.IsValid)
            {
                return Page();
            }

            Customer? customer = await _context.Customers
                .FirstOrDefaultAsync(c => c.CustomerId == TransactionRecord.CustomerId);

            DateTime startOfDay = TransactionRecord.TransactionDate.Date;
            DateTime endOfDay = startOfDay.AddDays(1);

            int transactionsTodayCount = await _context.TransactionRecords
                .CountAsync(t =>
                    t.CustomerId == TransactionRecord.CustomerId &&
                    t.TransactionDate >= startOfDay &&
                    t.TransactionDate < endOfDay);

            RiskScoringResult result = _riskScoringService.CalculateRisk(
                TransactionRecord,
                customer,
                transactionsTodayCount);

            TransactionRecord.RiskScore = result.RiskScore;
            TransactionRecord.RiskLevel = result.RiskLevel;
            TransactionRecord.FlagReason = result.FlagReason;
            TransactionRecord.CreatedAt = DateTime.Now;

            _context.TransactionRecords.Add(TransactionRecord);
            await _context.SaveChangesAsync();

            if (TransactionRecord.RiskScore >= 30)
            {
                RiskAlert alert = new()
                {
                    TransactionRecordId = TransactionRecord.TransactionRecordId,
                    RiskScore = TransactionRecord.RiskScore,
                    RiskLevel = TransactionRecord.RiskLevel,
                    Reason = TransactionRecord.FlagReason,
                    Status = "Open",
                    CreatedAt = DateTime.Now
                };

                _context.RiskAlerts.Add(alert);
            }

            AuditLog log = new()
            {
                Username = "System",
                Action = "Transaction Created",
                Details = $"Transaction {TransactionRecord.TransactionRecordId} created with risk level {TransactionRecord.RiskLevel}.",
                Timestamp = DateTime.Now
            };

            _context.AuditLogs.Add(log);

            await _context.SaveChangesAsync();

            return RedirectToPage("Index");
        }

        private async Task PopulateCustomersAsync()
        {
            var customers = await _context.Customers
                .OrderBy(c => c.FullName)
                .ToListAsync();

            CustomerSelectList = new SelectList(customers, "CustomerId", "FullName");

            Countries = CultureInfo.GetCultures(CultureTypes.SpecificCultures)
                .Select(culture => new RegionInfo(culture.Name).EnglishName)
                .Distinct()
                .OrderBy(country => country)
                .ToList();
        }
    }
}