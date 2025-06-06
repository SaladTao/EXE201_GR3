
using exe201.Models;
using Microsoft.EntityFrameworkCore;

namespace exe201.Repository.OrderDetail
{
    public class OrderDetailRepository : IOrderDetailRepository
    {
        private readonly EcommerceContext _context;
        public OrderDetailRepository(EcommerceContext context)
        {
            _context = context;
        }
        public List<Models.OrderDetail> GetOrderDetailsByProductId(int productId)
        {
            return _context.OrderDetails
            .Include(od => od.Product)
            .Where(od => od.ProductId == productId)
            .ToList();
        }
    }
}
