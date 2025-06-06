using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using exe201.Models;  // namespace chứa Product model

namespace exe201.Pages.Home
{
    public class DetailsProductModel : PageModel
    {
        private readonly EcommerceContext _context;

        public DetailsProductModel(EcommerceContext context)
        {
            _context = context;
        }

        [BindProperty(SupportsGet = true)]
        public int productId { get; set; }

        public Product Product { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            if (productId == 0)
            {
                return NotFound();
            }

            Product = await _context.Products
                .Include(p => p.Category) // Nếu có navigation property Category
                .FirstOrDefaultAsync(p => p.Id == productId);

            if (Product == null)
            {
                return NotFound(); // <- xử lý khi không tìm thấy sản phẩm
            }

            return Page();
        }
        public async Task<IActionResult> OnPostAddToCartAsync(int productId, string selectedSize)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
                return RedirectToPage("/Login/Index");

            // Tìm hoặc tạo giỏ hàng
            var cart = await _context.Carts
                .Include(c => c.CartItems)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart == null)
            {
                cart = new Cart
                {
                    UserId = userId.Value,
                    CartItems = new List<CartItem>()
                };
                _context.Carts.Add(cart);
                await _context.SaveChangesAsync(); // Lưu để có CartId
            }

            // Kiểm tra xem sản phẩm đã có trong giỏ hàng chưa
            var existingItem = cart.CartItems
                .FirstOrDefault(ci => ci.ProductId == productId && ci.Size == selectedSize);

            if (existingItem != null)
            {
                existingItem.Quantity += 1;
            }
            else
            {
                var newItem = new CartItem
                {
                    ProductId = productId,
                    Quantity = 1,
                    Size = selectedSize,
                    CartId = cart.Id
                };
                cart.CartItems.Add(newItem);
            }

            await _context.SaveChangesAsync();

            return RedirectToPage("/Cart/Index");
        }
    }
}
