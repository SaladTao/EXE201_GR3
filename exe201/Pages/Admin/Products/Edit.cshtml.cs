using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using exe201.Models;

namespace exe201.Pages.Admin.Products
{
    public class EditModel : PageModel
    {
        private readonly exe201.Models.EcommerceContext _context;

        public EditModel(exe201.Models.EcommerceContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Product Product { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product =  await _context.Products.FirstOrDefaultAsync(m => m.Id == id);
            if (product == null)
            {
                return NotFound();
            }
            Product = product;
           ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name");
            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {

            var productToUpdate = await _context.Products.FirstOrDefaultAsync(o => o.Id == Product.Id);

            if (productToUpdate == null)
            {
                return NotFound();
            }

            // Kiểm tra tên sản phẩm đã tồn tại (ngoại trừ chính sản phẩm đang chỉnh sửa)
            bool isDuplicateName = await _context.Products
                .AnyAsync(p => p.Name == Product.Name && p.Id != Product.Id);

            if (isDuplicateName)
            {
                ModelState.AddModelError("Product.Name", "Tên sản phẩm đã tồn tại, vui lòng chọn tên khác.");
                ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", Product.CategoryId);
                return Page();
            }

            productToUpdate.Name = Product.Name;
            productToUpdate.Description = Product.Description;
            productToUpdate.Price = Product.Price;
            productToUpdate.ImageUrl = Product.ImageUrl;
            productToUpdate.CategoryId = Product.CategoryId;
            productToUpdate.VirtualGift = Product.VirtualGift;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductExists(Product.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./Index");
        }

        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.Id == id);
        }
    }
}
