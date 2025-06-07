using exe201.Models;
using exe201.Repository.Order;
using Microsoft.AspNetCore.Mvc;

namespace exe201.Service
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        public OrderService(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        public Task<List<Order>> GetUserOrdersAsync(int userId)
        {
            return _orderRepository.GetOrdersByUserIdAsync(userId);
        }
    }
}
