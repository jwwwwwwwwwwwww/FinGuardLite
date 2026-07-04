using FinGuardLite.Data;
using FinGuardLite.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace FinGuardLite.Pages.RiskAlerts
{
    public class IndexModel : PageModel
    {
        private readonly FinGuardDbContext _context;

        public IndexModel(FinGuardDbContext context)
        {
            _context = context;
        }

        public IList<RiskAlert> RiskAlerts { get; set; } = new List<RiskAlert>();

        public async Task OnGetAsync()
        {
            RiskAlerts = await _context.RiskAlerts
                .Include(a => a.TransactionRecord)
                .ThenInclude(t => t.Customer)
                .OrderByDescending(a => a.CreatedAt)
                .ToListAsync();
        }
    }
}