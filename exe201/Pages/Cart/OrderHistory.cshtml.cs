using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using exe201.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace exe201.Pages.Cart
{
    public class OrderHistoryModel : PageModel
    {
        private readonly EcommerceContext _context;

        public OrderHistoryModel(EcommerceContext context)
        {
            _context = context;
        }

        public List<Order> Orders { get; set; }
        public string SuccessMessage { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToPage("/Login/Index");
            }

            // Lấy danh sách đơn hàng của người dùng, sắp xếp theo thời gian tạo giảm dần
            Orders = await _context.Orders
                .Where(o => o.UserId == userId)
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();

            // Lấy thông báo thành công từ TempData (nếu có)
            if (TempData["SuccessMessage"] != null)
            {
                SuccessMessage = TempData["SuccessMessage"].ToString();
            }

            return Page();
        }
    }
}
