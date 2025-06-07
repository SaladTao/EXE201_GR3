using exe201.Models;

namespace exe201.Repository.Order
{
    public interface IOrderRepository
    {
        Task<List<Models.Order>> GetOrdersByUserIdAsync(int userId);
    }
}
