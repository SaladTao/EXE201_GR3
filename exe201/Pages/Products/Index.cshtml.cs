using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using exe201.Data;
using exe201.Models;

namespace exe201.Pages.Products
{
    public class IndexModel : PageModel
    {
        private readonly exe201Context _context;

        public IndexModel(exe201Context context)
        {
            _context = context;
        }

        public List<Product> ProductList { get; set; } = new();

        public async Task OnGetAsync()
        {
            ProductList = await _context.Products
                .Include(p => p.Category) 
                .ToListAsync();
        }
    }
}
