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
    public class IndexModel : PageModel
    {
        private readonly exe201.Models.EcommerceContext _context;

        public IndexModel(exe201.Models.EcommerceContext context)
        {
            _context = context;
        }

        public IList<Product> Product { get; set; } = default!;
        public SelectList? Categories { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? SearchName { get; set; }

        [BindProperty(SupportsGet = true)]
        public int? CategoryId { get; set; }

        public int CurrentPage { get; set; } = 1;
        public int TotalPages { get; set; }
        public int PageSize { get; set; } = 2;

        public async Task OnGetAsync(int? pageNumber)
        {
            if (pageNumber.HasValue)
                CurrentPage = pageNumber.Value;

            var query = _context.Products
                .Include(p => p.Category)
                .AsQueryable();

            if (!string.IsNullOrEmpty(SearchName))
            {
                query = query.Where(p => p.Name.Contains(SearchName));
            }

            if (CategoryId.HasValue && CategoryId.Value != 0)
            {
                query = query.Where(p => p.CategoryId == CategoryId.Value);
            }

            int totalItems = await query.CountAsync();
            TotalPages = (int)Math.Ceiling(totalItems / (double)PageSize);

            Product = await query
                .OrderByDescending(p => p.CreatedAt)
                .Skip((CurrentPage - 1) * PageSize)
                .Take(PageSize)
                .ToListAsync();

            Categories = new SelectList(await _context.Categories.ToListAsync(), "Id", "Name");
        }
    }
}
