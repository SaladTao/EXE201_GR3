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
                return NotFound();
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAddToCartAsync(int productId)
        {
            // Kiểm tra đăng nhập
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToPage("/Login/Index");
            }

            // Tìm hoặc tạo giỏ hàng cho user
            var cart = await _context.Carts
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart == null)
            {
                cart = new exe201.Models.Cart { UserId = userId.Value, CreatedAt = DateTime.Now };
                _context.Carts.Add(cart);
                await _context.SaveChangesAsync();
            }

            // Kiểm tra sản phẩm đã có trong giỏ hàng chưa
            var cartItem = await _context.CartItems
                .FirstOrDefaultAsync(ci => ci.CartId == cart.Id && ci.ProductId == productId);

            if (cartItem == null)
            {
                // Thêm sản phẩm mới vào giỏ hàng
                cartItem = new CartItem
                {
                    CartId = cart.Id,
                    ProductId = productId,
                    Quantity = 1
                };
                _context.CartItems.Add(cartItem);
            }
            else
            {
                // Tăng số lượng nếu sản phẩm đã có trong giỏ
                cartItem.Quantity++;
            }

            await _context.SaveChangesAsync();

            // Chuyển hướng về trang chi tiết sản phẩm với thông báo thành công
            TempData["SuccessMessage"] = "Đã thêm sản phẩm vào giỏ hàng!";
            return RedirectToPage(new { productId = productId });
        }
    }
}
