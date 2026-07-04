using FinGuardLite.Data;
using FinGuardLite.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace FinGuardLite.Pages.AuditLogs
{
    public class IndexModel : PageModel
    {
        private readonly FinGuardDbContext _context;

        public IndexModel(FinGuardDbContext context)
        {
            _context = context;
        }

        public IList<AuditLog> AuditLogs { get; set; } = new List<AuditLog>();

        public async Task OnGetAsync()
        {
            AuditLogs = await _context.AuditLogs
                .OrderByDescending(a => a.Timestamp)
                .ToListAsync();
        }
    }
}