using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using exe201.Data;
using exe201.Models;
using Cart = exe201.Models.Cart;
using CartItem = exe201.Models.CartItem;

namespace exe201.Pages.Products
{
    public class IndexModel : PageModel
    {
        private readonly exe201Context _context;

        public IndexModel(exe201Context context)
        {
            _context = context;
        }

        public List<Product> ProductList { get; set; } = new();

        public async Task OnGetAsync()
        {
            ProductList = await _context.Products
                .Include(p => p.Category)
                .ToListAsync();
        }

        public async Task<IActionResult> OnPostAddToCartAsync(int id)
        {
            var userIdStr = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userIdStr))
            {
                return RedirectToPage("/Login/Index");
            }

            int userId = int.Parse(userIdStr);

            // Tìm hoặc tạo giỏ hàng của user
            var cart = await _context.Carts
                .Include(c => c.CartItems)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart == null)
            {
                cart = new exe201.Models.Cart
                {
                    UserId = userId,
                    CartItems = new List<exe201.Models.CartItem>()
                };
                _context.Carts.Add(cart);
                await _context.SaveChangesAsync(); // tạo CartId
            }

            // Kiểm tra sản phẩm đã có trong giỏ chưa
            var existingItem = cart.CartItems.FirstOrDefault(i => i.ProductId == id);
            if (existingItem != null)
            {
                existingItem.Quantity += 1;
            }
            else
            {
                cart.CartItems.Add(new CartItem
                {
                    ProductId = id,
                    Quantity = 1
                });
            }

            await _context.SaveChangesAsync();
            return RedirectToPage();
        }
    }
}
