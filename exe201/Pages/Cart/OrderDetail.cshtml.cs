using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using exe201.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace exe201.Pages.Cart
{
    public class OrderDetailModel : PageModel
    {
        private readonly EcommerceContext _context;

        public OrderDetailModel(EcommerceContext context)
        {
            _context = context;
        }

        [BindProperty(SupportsGet = true)]
        public int OrderId { get; set; }

        public Order Order { get; set; }
        public List<OrderDetail> OrderDetails { get; set; }
        public User User { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToPage("/Login/Index");
            }

            // Lấy thông tin đơn hàng
            Order = await _context.Orders
                .FirstOrDefaultAsync(o => o.Id == OrderId && o.UserId == userId);

            if (Order == null)
            {
                return NotFound();
            }

            // Lấy chi tiết đơn hàng
            OrderDetails = await _context.OrderDetails
                .Include(od => od.Product)
                .Where(od => od.OrderId == OrderId)
                .ToListAsync();

            // Lấy thông tin người dùng
            User = await _context.Users
                .Include(u => u.Profile)
                .FirstOrDefaultAsync(u => u.Id == userId);

            return Page();
        }
    }
}
