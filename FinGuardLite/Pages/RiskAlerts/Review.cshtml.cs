using FinGuardLite.Data;
using FinGuardLite.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace FinGuardLite.Pages.RiskAlerts
{
    public class ReviewModel : PageModel
    {
        private readonly FinGuardDbContext _context;

        public ReviewModel(FinGuardDbContext context)
        {
            _context = context;
        }

        public RiskAlert? RiskAlert { get; set; }

        [BindProperty]
        public int RiskAlertId { get; set; }

        [BindProperty]
        public string Status { get; set; } = "Open";

        [BindProperty]
        public string AnalystComment { get; set; } = string.Empty;

        public async Task<IActionResult> OnGetAsync(int id)
        {
            RiskAlert = await _context.RiskAlerts
                .Include(a => a.TransactionRecord)
                .ThenInclude(t => t.Customer)
                .FirstOrDefaultAsync(a => a.RiskAlertId == id);

            if (RiskAlert == null)
            {
                return NotFound();
            }

            RiskAlertId = RiskAlert.RiskAlertId;
            Status = RiskAlert.Status;
            AnalystComment = RiskAlert.AnalystComment ?? string.Empty;

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            RiskAlert? alert = await _context.RiskAlerts
                .FirstOrDefaultAsync(a => a.RiskAlertId == RiskAlertId);

            if (alert == null)
            {
                return NotFound();
            }

            string safeStatus = string.IsNullOrWhiteSpace(Status)
                ? "Open"
                : Status;

            string safeComment = string.IsNullOrWhiteSpace(AnalystComment)
                ? "No analyst comment provided."
                : AnalystComment.Trim();

            alert.Status = safeStatus;
            alert.AnalystComment = safeComment;
            alert.UpdatedAt = DateTime.Now;

            string username = HttpContext.Session.GetString("Role") ?? "Analyst";

            AuditLog log = new()
            {
                Username = username,
                Action = "Risk Alert Reviewed",
                Details = $"Risk alert {alert.RiskAlertId} status changed to {safeStatus}. Comment: {safeComment}",
                Timestamp = DateTime.Now
            };

            _context.AuditLogs.Add(log);

            await _context.SaveChangesAsync();

            return RedirectToPage("Index");
        }
    }
}