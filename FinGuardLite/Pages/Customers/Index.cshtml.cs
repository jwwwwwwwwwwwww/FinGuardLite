using FinGuardLite.Data;
using FinGuardLite.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace FinGuardLite.Pages.Customers
{
    public class IndexModel : PageModel
    {
        private readonly FinGuardDbContext _context;

        public IndexModel(FinGuardDbContext context)
        {
            _context = context;
        }

        public IList<Customer> Customers { get; set; } = new List<Customer>();

        public async Task OnGetAsync()
        {
            Customers = await _context.Customers
                .OrderBy(c => c.FullName)
                .ToListAsync();
        }
    }
}