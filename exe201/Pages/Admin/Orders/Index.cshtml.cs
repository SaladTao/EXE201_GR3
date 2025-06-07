using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using exe201.Models;

namespace exe201.Pages.Admin.Orders
{
    public class IndexModel : PageModel
    {
        private readonly exe201.Models.EcommerceContext _context;

        public IndexModel(exe201.Models.EcommerceContext context)
        {
            _context = context;
        }

        public IList<Order> Order { get; set; } = default!;

        [BindProperty(SupportsGet = true)]
        public string? SearchEmail { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? StatusFilter { get; set; }

        public int CurrentPage { get; set; } = 1;
        public int TotalPages { get; set; }
        public int PageSize { get; set; } = 2;

        public async Task<IActionResult> OnGetAsync(int? pageNumber)
        {
            var userIdStr = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userIdStr))
            {
                return RedirectToPage("/Login/Index");
            }

            if (pageNumber.HasValue)
                CurrentPage = pageNumber.Value;

            var query = _context.Orders
                .Include(o => o.User)
                .AsQueryable();

            if (!string.IsNullOrEmpty(SearchEmail))
            {
                query = query.Where(o => o.User != null && o.User.Email.Contains(SearchEmail));
            }

            if (!string.IsNullOrEmpty(StatusFilter))
            {
                query = query.Where(o => o.Status == StatusFilter);
            }

            int totalItems = await query.CountAsync();
            TotalPages = (int)Math.Ceiling(totalItems / (double)PageSize);

            Order = await query
                .OrderByDescending(o => o.CreatedAt)
                .Skip((CurrentPage - 1) * PageSize)
                .Take(PageSize)
                .ToListAsync();

            return Page(); // Don't forget this!
        }

    }
}
