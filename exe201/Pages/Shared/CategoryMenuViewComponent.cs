using Microsoft.AspNetCore.Mvc;
using exe201.Models; // sửa namespace nếu khác
using System.Linq;

namespace exe201.ViewComponents
{
    public class CategoryMenuViewComponent : ViewComponent
    {
        private readonly EcommerceContext _context;

        public CategoryMenuViewComponent(EcommerceContext context)
        {
            _context = context;
        }

        public IViewComponentResult Invoke()
        {
            var categories = _context.Categories.ToList(); // lấy danh sách category
            return View(categories); // trả về view với model là danh sách category
        }
    }
}
