using exe201.Models;
using Microsoft.EntityFrameworkCore;

namespace exe201.Repository.Order
{
    public class OrderRepository : IOrderRepository
    {
        private readonly EcommerceContext _context;
        public OrderRepository(EcommerceContext context)
        {
            _context = context;
        }

        public async Task<List<Models.Order>> GetOrdersByUserIdAsync(int userId)
        {
            return await _context.Orders
    .Include(o => o.OrderDetails)
    .Where(o => o.UserId == userId)
    .OrderByDescending(o => o.CreatedAt)
    .ToListAsync();
        }
    }
}
