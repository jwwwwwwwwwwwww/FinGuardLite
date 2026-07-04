using FinGuardLite.Data;
using FinGuardLite.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace FinGuardLite.Pages.Transactions
{
    public class IndexModel : PageModel
    {
        private readonly FinGuardDbContext _context;

        public IndexModel(FinGuardDbContext context)
        {
            _context = context;
        }

        public IList<TransactionRecord> TransactionRecords { get; set; } = new List<TransactionRecord>();

        public async Task OnGetAsync()
        {
            TransactionRecords = await _context.TransactionRecords
                .Include(t => t.Customer)
                .OrderByDescending(t => t.TransactionDate)
                .ToListAsync();
        }
    }
}