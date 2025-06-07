using exe201.Models;

namespace exe201.Repository.OrderDetail
{
    public interface IOrderDetailRepository
    {
        List<Models.OrderDetail> GetOrderDetailsByProductId(int productId);
    }
}
