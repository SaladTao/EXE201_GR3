using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using exe201.Models;
using Microsoft.EntityFrameworkCore;

namespace exe201.Pages.Admin.Products
{
    public class CreateModel : PageModel
    {
        private readonly exe201.Models.EcommerceContext _context;

        public CreateModel(exe201.Models.EcommerceContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
        ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name");
            return Page();
        }

        [BindProperty]
        public Product Product { get; set; } = default!;

        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            // Kiểm tra trùng tên sản phẩm
            bool isDuplicateName = await _context.Products.AnyAsync(p => p.Name == Product.Name);
            if (isDuplicateName)
            {
                ModelState.AddModelError("Product.Name", "Tên sản phẩm đã tồn tại, vui lòng chọn tên khác.");
                ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", Product.CategoryId);
                return Page();
            }

            _context.Products.Add(Product);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
