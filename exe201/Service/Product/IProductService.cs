using exe201.Models;

namespace exe201.Service.Product
{
    public interface IProductService
    {
        Models.Product GetProductById(int id);
        List<OrderDetail> GetOrderDetailsForProduct(int productId);
    }
}
