using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using exe201.Models;
using Microsoft.EntityFrameworkCore;

namespace exe201.Pages.Admin.Categories
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
            return Page();
        }

        [BindProperty]
        public Category Category { get; set; } = default!;

        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {


            // Kiểm tra tên danh mục đã tồn tại
            bool nameExists = await _context.Categories
                .AnyAsync(c => c.Name.ToLower() == Category.Name.ToLower().Trim());

            if (nameExists)
            {
                ModelState.AddModelError("Category.Name", "Tên danh mục đã tồn tại.");
                return Page();
            }

            _context.Categories.Add(Category);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }

    }
}
