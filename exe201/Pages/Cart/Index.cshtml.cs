using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using exe201.Models;
using Microsoft.EntityFrameworkCore;

namespace exe201.Pages.Cart
{
    public class IndexModel : PageModel
    {
        private readonly EcommerceContext _context;

        public IndexModel(EcommerceContext context)
        {
            _context = context;
        }

        public IList<CartItem> CartItems { get; set; }
        public decimal TotalAmount { get; set; }

        public void OnGet()
        {
            var userId = 1; // Giả sử ID người dùng hiện tại là 1 (có thể lấy từ Session hoặc Cookie)

            // Lấy các mục trong giỏ hàng của người dùng
            CartItems = _context.CartItems
                                 .Where(ci => ci.Cart.UserId == userId)
                                 .Include(ci => ci.Product)
                                 .ToList();

            // Tính tổng tiền của giỏ hàng
            TotalAmount = CartItems.Sum(ci => ci.Quantity * ci.Product.Price);
        }
    }
}
