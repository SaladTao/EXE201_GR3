using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using exe201.Models;

namespace exe201.Pages.Login
{
    public class IndexModel : PageModel
    {
        private readonly EcommerceContext _context;

        public IndexModel(EcommerceContext context)
        {
            _context = context;
        }

        [BindProperty]
        public LoginInputModel Input { get; set; }

        public string? ErrorMessage { get; set; }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == Input.Email && u.Password == Input.Password);

            if (user == null)
            {
                ErrorMessage = "Sai mật khẩu hoặc tài khoản.";
                return Page();
            }

            // Lưu thông tin user vào session (ví dụ lưu UserId và Username)
            HttpContext.Session.SetInt32("UserId", user.Id);
            HttpContext.Session.SetString("Username", user.Username ?? "");

            return RedirectToPage("/Home/Index");
        }


        public class LoginInputModel
        {
            [Required(ErrorMessage = "Vui lòng nhập Email")]
            [EmailAddress(ErrorMessage = "Email không đúng định dạng")]
            public string Email { get; set; }

            [Required, DataType(DataType.Password)]
            public string Password { get; set; }
        }
    }
}
