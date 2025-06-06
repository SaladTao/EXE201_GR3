using exe201.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace exe201.Pages.Cart
{
    public class IndexModel : PageModel
    {
        private readonly EcommerceContext _context;

        public IndexModel(EcommerceContext context)
        {
            _context = context;
        }

        public List<CartItem> CartItems { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return RedirectToPage("/Login/Index");

            var cart = await _context.Carts
                            .Include(c => c.CartItems)
                                .ThenInclude(ci => ci.Product)
                            .FirstOrDefaultAsync(c => c.UserId == userId);

            CartItems = cart?.CartItems?.ToList() ?? new List<CartItem>();

            return Page();
        }
    }
}
