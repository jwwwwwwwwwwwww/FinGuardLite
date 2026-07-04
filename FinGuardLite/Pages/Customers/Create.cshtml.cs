using FinGuardLite.Data;
using FinGuardLite.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FinGuardLite.Pages.Customers
{
    public class CreateModel : PageModel
    {
        private readonly FinGuardDbContext _context;

        public CreateModel(FinGuardDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Customer Customer { get; set; } = new();

        public void OnGet()
        {
            Customer.Country = "Singapore";
            Customer.RiskProfile = "Low";
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            Customer.CreatedAt = DateTime.Now;

            _context.Customers.Add(Customer);

            _context.AuditLogs.Add(new AuditLog
            {
                Username = "Admin",
                Action = "Customer Created",
                Details = $"Customer profile created for {Customer.FullName}.",
                Timestamp = DateTime.Now
            });

            await _context.SaveChangesAsync();

            return RedirectToPage("/Customers/Index");
        }
    }
}