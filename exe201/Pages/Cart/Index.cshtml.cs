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

        public List<CartItem> CartItems { get; set; } = new List<CartItem>();
        public decimal TotalAmount { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToPage("/Login/Index");
            }

            var cart = await _context.Carts.FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart == null)
            {
                CartItems = new List<CartItem>();
                return Page();
            }

            CartItems = await _context.CartItems
                .Include(ci => ci.Product)
                .Where(ci => ci.CartId == cart.Id)
                .ToListAsync();

            if (CartItems == null)
                CartItems = new List<CartItem>();

            TotalAmount = CartItems.Sum(ci => ci.Product.Price * ci.Quantity);

            return Page();
        }

        public async Task<IActionResult> OnPostDeleteAsync(int cartItemId)
        {
            var cartItem = await _context.CartItems.FindAsync(cartItemId);
            if (cartItem != null)
            {
                _context.CartItems.Remove(cartItem);
                await _context.SaveChangesAsync();
            }
            // Sau khi xóa, luôn redirect về trang để OnGetAsync lấy lại dữ liệu mới nhất
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostUpdateQuantityAsync(int cartItemId, int quantity)
        {
            var cartItem = await _context.CartItems.FindAsync(cartItemId);
            if (cartItem != null)
            {
                cartItem.Quantity = quantity;
                await _context.SaveChangesAsync();
            }
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostCheckoutAsync(int[] selectedItems)
        {
            if (selectedItems == null || !selectedItems.Any())
            {
                return RedirectToPage();
            }

            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToPage("/Login/Index");
            }

            // Lưu danh sách sản phẩm đã chọn vào session
            HttpContext.Session.SetString("SelectedCartItems", string.Join(",", selectedItems));

            return RedirectToPage("/Cart/Checkout");
        }
    }
}
