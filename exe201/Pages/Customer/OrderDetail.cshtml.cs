using exe201.Models;
using exe201.Service.Product;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.RegularExpressions;

namespace exe201.Pages.Customer
{
    public class OrderDetailModel : PageModel
    {
        private readonly ProductService _productService;

        public OrderDetailModel(ProductService productService)
        {
            _productService = productService;
        }

        public Product Product { get; set; }
        public List<OrderDetail> OrderDetails { get; set; }

        public IActionResult OnGet(int id)
        {
            Product = _productService.GetProductById(id);
            if (Product == null)
            {
                return NotFound();
            }

            OrderDetails = _productService.GetOrderDetailsForProduct(id);
            return Page();
        }
    }
}
