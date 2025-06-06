using exe201.Models;
using Microsoft.AspNetCore.Mvc;

namespace exe201.Service
{
    public interface IOrderService
    {
        Task<List<Order>> GetUserOrdersAsync(int userId);
    }
}
