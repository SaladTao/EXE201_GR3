using exe201.Models;

namespace exe201.Repository.Product
{
    public interface IProductRepository
    {
        exe201.Models.Product GetProductById(int productId);
    }
}
