using exe201.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace exe201.Pages.Products
{
    public class CreateModel : PageModel
    {
        private readonly EcommerceContext _context;

        public CreateModel(EcommerceContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Product Product { get; set; }

        public SelectList CategoryList { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            // Load danh mục từ DB để hiển thị drop-down
            var categories = await _context.Categories.ToListAsync();
            CategoryList = new SelectList(categories, "Id", "Name");
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                // Nếu lỗi form
                var categories = await _context.Categories.ToListAsync();
                CategoryList = new SelectList(categories, "Id", "Name");
                return Page();
            }

            Product.CreatedAt = DateTime.Now;
            _context.Products.Add(Product);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
