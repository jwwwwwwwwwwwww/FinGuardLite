using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FinGuardLite.Pages
{
    public class LoginModel : PageModel
    {
        [BindProperty]
        public string Username { get; set; } = string.Empty;

        [BindProperty]
        public string Password { get; set; } = string.Empty;

        public string ErrorMessage { get; set; } = string.Empty;

        public IActionResult OnGet()
        {
            // Prevent logged-in users from going back to the login page
            if (!string.IsNullOrEmpty(HttpContext.Session.GetString("Username")))
            {
                return RedirectToPage("/Index");
            }

            return Page();
        }

        public IActionResult OnPost()
        {
            if (Username == "analyst" && Password == "password123")
            {
                SetLoginSession("analyst", "Risk Analyst");
                return RedirectToPage("/Index");
            }

            if (Username == "compliance" && Password == "password123")
            {
                SetLoginSession("compliance", "Compliance Officer");
                return RedirectToPage("/Index");
            }

            if (Username == "admin" && Password == "password123")
            {
                SetLoginSession("admin", "Admin");
                return RedirectToPage("/Index");
            }

            ErrorMessage = "Invalid username or password.";
            return Page();
        }

        private void SetLoginSession(string username, string role)
        {
            HttpContext.Session.SetString("Username", username);
            HttpContext.Session.SetString("Role", role);
        }
    }
}