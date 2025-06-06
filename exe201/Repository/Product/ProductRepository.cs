
using exe201.Models;

namespace exe201.Repository.Product
{
    public class ProductRepository : IProductRepository
    {
        private readonly EcommerceContext _context;
        public ProductRepository(EcommerceContext context)
        {
            _context = context;
        }
        public Models.Product GetProductById(int productId)
        {
            return _context.Products.FirstOrDefault(p => p.Id == productId);
        }
    }
}
