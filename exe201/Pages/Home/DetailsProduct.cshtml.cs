using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using exe201.Models;
using System.ComponentModel.DataAnnotations;  // namespace chứa Product model

namespace exe201.Pages.Home
{
    public class DetailsProductModel : PageModel
    {
        private readonly EcommerceContext _context;

        public DetailsProductModel(EcommerceContext context)
        {
            _context = context;
        }
        [BindProperty]
        [Range(1, 100, ErrorMessage = "Số lượng phải từ 1 đến 100.")]
        public int Quantity { get; set; }
        [BindProperty(SupportsGet = true)]
        public int productId { get; set; }

        public Product Product { get; set; }
        public List<Size> Sizes { get; set; }

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

            // Get all sizes
            Sizes = await _context.Sizes.ToListAsync();

            return Page();
        }
        public async Task<IActionResult> OnPostAddToCartAsync(int productId, int sizeId)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToPage("/Login/Index");
            }

            // Tìm hoặc tạo giỏ hàng
            var cart = await _context.Carts.FirstOrDefaultAsync(c => c.UserId == userId);
            if (cart == null)
            {
                cart = new exe201.Models.Cart { UserId = userId.Value, CreatedAt = DateTime.Now };
                _context.Carts.Add(cart);
                await _context.SaveChangesAsync();
            }

            // Tìm sản phẩm có cùng productId + sizeId
            var existingItem = await _context.CartItems.FirstOrDefaultAsync(ci =>
                ci.CartId == cart.Id && ci.ProductId == productId && ci.SizeId == sizeId);

            if (existingItem == null)
            {
                var cartItem = new CartItem
                {
                    CartId = cart.Id,
                    ProductId = productId,
                    SizeId = sizeId,
                    Quantity = Quantity > 0 ? Quantity : 1
                };
                _context.CartItems.Add(cartItem);
            }
            else
            {
                existingItem.Quantity += Quantity > 0 ? Quantity : 1;
            }

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Đã thêm sản phẩm vào giỏ hàng!";
            return RedirectToPage(new { productId });
        }
    }
}
