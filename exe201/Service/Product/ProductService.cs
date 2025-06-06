
using exe201.Models;
using exe201.Repository.OrderDetail;
using exe201.Repository.Product;

namespace exe201.Service.Product
{
    public class ProductService : IProductService
    {
        private IProductRepository _productRepo;
        private readonly IOrderDetailRepository _orderDetailRepo;
        public ProductService(IProductRepository productRepo, IOrderDetailRepository orderDetailRepo)
        {
            _productRepo = productRepo;
            _orderDetailRepo = orderDetailRepo;
        }
        public Models.Product GetProductById(int id)
        {
            return _productRepo.GetProductById(id);
        }

        public List<OrderDetail> GetOrderDetailsForProduct(int productId)
        {
            return _orderDetailRepo.GetOrderDetailsByProductId(productId);
        }
    }
}
