using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using exe201.Data;
using exe201.Models;

namespace exe201.Pages.Orders
{
    public class IndexModel : PageModel
    {
        private readonly exe201Context _context;

        public IndexModel(exe201Context context)
        {
            _context = context;
        }

        public List<Order> Orders { get; set; } = new();

        public async Task<IActionResult> OnGetAsync()
        {
            var userIdStr = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userIdStr))
                return RedirectToPage("/Login/Index");

            int userId = int.Parse(userIdStr);

            Orders = await _context.Orders
                .Where(o => o.UserId == userId)
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();

            return Page();
        }
    }
}
