using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using exe201.Models;

namespace exe201.Pages.Admin.Users
{
    public class IndexModel : PageModel
    {
        private readonly exe201.Models.EcommerceContext _context;

        public IndexModel(exe201.Models.EcommerceContext context)
        {
            _context = context;
        }

        public IList<User> User { get; set; } = default!;

        [BindProperty(SupportsGet = true)]
        public string? SearchTerm { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? RoleFilter { get; set; }
        public List<string> Roles { get; set; } = new(); // Danh sách role dùng cho dropdown

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

            var query = _context.Users
                .Include(u => u.Profile)
                .AsQueryable();

            // Lọc theo SearchTerm
            if (!string.IsNullOrWhiteSpace(SearchTerm))
            {
                query = query.Where(u =>
                    u.Username.Contains(SearchTerm) ||
                    u.Email.Contains(SearchTerm) ||
                    (u.Profile != null && u.Profile.FullName.Contains(SearchTerm)));
            }

            // Lọc theo Role
            if (!string.IsNullOrWhiteSpace(RoleFilter))
            {
                query = query.Where(u => u.Role == RoleFilter);
            }

            // Lấy tất cả role duy nhất để hiển thị trong dropdown
            Roles = await _context.Users
                .Select(u => u.Role)
                .Distinct()
                .ToListAsync();

            // Phân trang
            int totalItems = await query.CountAsync();
            TotalPages = (int)Math.Ceiling(totalItems / (double)PageSize);

            User = await query
                .OrderByDescending(u => u.CreatedAt)
                .Skip((CurrentPage - 1) * PageSize)
                .Take(PageSize)
                .ToListAsync();

            return Page(); 
        }


    }
}
