using exe201.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Linq;

namespace exe201.Pages.Home
{
    public class FilterModel : PageModel
    {
        private readonly EcommerceContext context;

        public FilterModel(EcommerceContext _context)
        {
            context = _context;
        }

        public List<Product> products { get; set; }
        public List<Category> categories { get; set; }

        [BindProperty(SupportsGet = true)]
        public string Keyword { get; set; }

        [BindProperty(SupportsGet = true)]
        public int? categoryId { get; set; } // Thêm dòng này để bind categoryId từ URL

        public void OnGet()
        {
            categories = context.Categories.ToList();

            var query = context.Products.AsQueryable();

            if (!string.IsNullOrEmpty(Keyword))
            {
                query = query.Where(p => p.Name.Contains(Keyword));
            }

            if (categoryId.HasValue)
            {
                query = query.Where(p => p.CategoryId == categoryId);
            }

            products = query.ToList();
        }
    }
}
