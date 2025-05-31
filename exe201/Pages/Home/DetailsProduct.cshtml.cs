using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using exe201.Models;  // namespace chứa Product model

namespace exe201.Pages.Home
{
    public class DetailsProductModel : PageModel
    {
        private readonly EcommerceContext _context;

        public DetailsProductModel(EcommerceContext context)
        {
            _context = context;
        }

        [BindProperty(SupportsGet = true)]
        public int productId { get; set; }

        public Product Product { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            if (productId == 0)
            {
                return NotFound();
            }

            Product = await _context.Products
                .Include(p => p.Category) // Nếu có navigation property Category
                .FirstOrDefaultAsync(p => p.Id == productId);

            if (Product == null)
            {
                return NotFound();
            }

            return Page();
        }
    }
}
