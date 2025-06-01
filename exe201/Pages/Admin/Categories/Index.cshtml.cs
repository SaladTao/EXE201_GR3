using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using exe201.Models;

namespace exe201.Pages.Admin.Categories
{
    public class IndexModel : PageModel
    {
        private readonly exe201.Models.EcommerceContext _context;

        public IndexModel(exe201.Models.EcommerceContext context)
        {
            _context = context;
        }

        public IList<Category> Category { get; set; } = default!;
        [BindProperty(SupportsGet = true)]
        public string? SearchString { get; set; }

        public int CurrentPage { get; set; } = 1;
        public int TotalPages { get; set; }
        public int PageSize { get; set; } = 2;

        public async Task OnGetAsync(int? pageNumber)
        {
            if (pageNumber.HasValue)
                CurrentPage = pageNumber.Value;

            var query = _context.Categories.AsQueryable();

            if (!string.IsNullOrEmpty(SearchString))
            {
                query = query.Where(c => c.Name.Contains(SearchString));
            }

            int totalItems = await query.CountAsync();
            TotalPages = (int)Math.Ceiling(totalItems / (double)PageSize);

            Category = await query
                .OrderByDescending(c => c.CreatedAt)
                .Skip((CurrentPage - 1) * PageSize)
                .Take(PageSize)
                .ToListAsync();
        }
    }
}
