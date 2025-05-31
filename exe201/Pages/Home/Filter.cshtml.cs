using exe201.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace exe201.Pages.Home
{
    public class FilterModel : PageModel
    {
        private readonly EcommerceContext context;
        public FilterModel(EcommerceContext _context)
        {
            context = _context;
        }
        public Product product { get; set; }
        public List<Product> products { get; set; }
        public List<Category> categories { get; set; }
        public void OnGet(int id)
        {
           
            products = context.Products.ToList();
            categories = context.Categories.ToList();

        }
    }
}
