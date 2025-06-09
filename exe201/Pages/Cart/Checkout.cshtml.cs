using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using exe201.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace exe201.Pages.Cart
{
    public class CheckoutModel : PageModel
    {
        private readonly EcommerceContext _context;

        public CheckoutModel(EcommerceContext context)
        {
            _context = context;
        }

        [BindProperty]
        public string FullName { get; set; }
        [BindProperty]
        public string Address { get; set; }
        [BindProperty]
        public string Phone { get; set; }

        public List<CartItem> CartItems { get; set; }
        public decimal Total { get; set; }

        public string SuccessMessage { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToPage("/Login/Index");
            }

            // Lấy danh sách sản phẩm đã chọn từ session
            var selectedItemsStr = HttpContext.Session.GetString("SelectedCartItems");
            if (string.IsNullOrEmpty(selectedItemsStr))
            {
                return RedirectToPage("/Cart/Index");
            }

            var selectedItemIds = selectedItemsStr.Split(',').Select(int.Parse).ToList();

            // Get user profile information
            var userProfile = await _context.UserProfiles
                .FirstOrDefaultAsync(u => u.UserId == userId);

            if (userProfile != null)
            {
                FullName = userProfile.FullName;
                Address = userProfile.Address;
                Phone = userProfile.Phone;
            }

            // Get selected cart items
            CartItems = await _context.CartItems
                .Include(ci => ci.Product)
                .Where(ci => selectedItemIds.Contains(ci.Id))
                .ToListAsync();

            if (CartItems != null)
            {
                Total = CartItems.Sum(item => item.Product.Price * item.Quantity);
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            int userId = HttpContext.Session.GetInt32("UserId")
            ?? throw new InvalidOperationException("UserId không tồn tại trong Session");

            if (userId == null)
            {
                return RedirectToPage("/Login/Index");
            }

            if (!ModelState.IsValid)
            {
                return Page();
            }

            // Lấy danh sách sản phẩm đã chọn từ session
            var selectedItemsStr = HttpContext.Session.GetString("SelectedCartItems");
            if (string.IsNullOrEmpty(selectedItemsStr))
            {
                return RedirectToPage("/Cart/Index");
            }

            var selectedItemIds = selectedItemsStr.Split(',').Select(int.Parse).ToList();

            // Get selected cart items
            var cartItems = await _context.CartItems
                .Include(ci => ci.Product)
                .Where(ci => selectedItemIds.Contains(ci.Id))
                .ToListAsync();

            if (!cartItems.Any())
            {
                return RedirectToPage("/Cart/Index");
            }

            // Chỉ tạo Order với các trường cần thiết
            var order = new Order
            {
                UserId = userId,
                TotalAmount = cartItems.Sum(item => item.Product.Price * item.Quantity),
                Status = OrderStatus.Pending.ToString(),
                CreatedAt = DateTime.UtcNow.AddHours(7) // Giờ Việt Nam (UTC+7)
            };

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            // Chỉ tạo OrderDetail với các trường cần thiết
            foreach (var item in cartItems)
            {
                var orderDetail = new OrderDetail
                {
                    OrderId = order.Id,
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    Price = item.Product.Price
                };
                _context.OrderDetails.Add(orderDetail);
            }

            // Remove selected items from cart
            _context.CartItems.RemoveRange(cartItems);
            await _context.SaveChangesAsync();

            // Clear selected items from session
            HttpContext.Session.Remove("SelectedCartItems");

            // Chuyển hướng đến trang OrderHistory sau khi đặt hàng thành công
            TempData["SuccessMessage"] = "Đặt hàng thành công! Cảm ơn bạn đã mua hàng.";
            return RedirectToPage("/Cart/OrderHistory");
        }
    }
}
