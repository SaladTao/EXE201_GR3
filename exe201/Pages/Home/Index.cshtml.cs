using exe201.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace exe201.Pages.Home
{
    public class IndexModel : PageModel
    {
        private readonly EcommerceContext context;
        public IndexModel(EcommerceContext _context)
        {
            context = _context;
        }

        public Product product { get; set; }
        public List<Product> products { get; set; }
        public List<Category> categories { get; set; }
        public void OnGet()
        {
            product = context.Products.FirstOrDefault(p => p.Id == 3);
            if (product == null)
            {
                // Gán dữ liệu giả hoặc xử lý để không bị null
                product = new Product
                {
                    Name = "Không tìm thấy sản phẩm",
                    Description = "Sản phẩm không tồn tại",
                    Price = 0
                };
            }

            products = context.Products.ToList();   
            categories = context.Categories.ToList();

        }
    }
}
