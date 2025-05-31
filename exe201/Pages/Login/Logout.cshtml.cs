using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace exe201.Pages.Login
{
    public class LogoutModel : PageModel
    {
        public IActionResult OnGet()
        {
            // Xóa tất cả session liên quan đăng nhập
            HttpContext.Session.Remove("UserId");
            HttpContext.Session.Remove("Username");

            // Hoặc xóa hết session
            // HttpContext.Session.Clear();

            // Chuyển về trang Login hoặc trang chủ
            return RedirectToPage("/Login/Index");
        }
    }
}
