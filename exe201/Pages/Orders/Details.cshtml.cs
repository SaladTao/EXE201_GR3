using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using exe201.Models;

namespace exe201.Pages.Orders
{
    public class DetailsModel : PageModel
    {
        private readonly EcommerceContext _context;

        public DetailsModel(EcommerceContext context)
        {
            _context = context;
        }

        public Order Order { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var userIdStr = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userIdStr))
                return RedirectToPage("/Login/Index");

            int userId = int.Parse(userIdStr);

            Order = await _context.Orders
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Product)
                .FirstOrDefaultAsync(o => o.Id == id && o.UserId == userId);

            if (Order == null)
                return NotFound();

            return Page();
        }
    }
}
