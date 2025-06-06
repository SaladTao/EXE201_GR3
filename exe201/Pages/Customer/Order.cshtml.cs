using exe201.Models;
using exe201.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace exe201.Pages.Customer
{
    public class OrderModel : PageModel
    {
        private readonly OrderService _orderService;
        public OrderModel(OrderService orderService)
        {
            _orderService = orderService;
        }
        public List<Order> Orders { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            Orders = await _orderService.GetUserOrdersAsync(2);
            if (Orders == null || !Orders.Any())
            {
                return NotFound("No orders found.");
            }
            return Page();

        }
    }
}
