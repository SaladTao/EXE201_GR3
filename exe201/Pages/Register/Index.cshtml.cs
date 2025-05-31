using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using exe201.Models;
using System.Linq;

namespace exe201.Pages.Register
{
    public class IndexModel : PageModel
    {
        private readonly EcommerceContext _context;

        public IndexModel(EcommerceContext context)
        {
            _context = context;
        }

        [BindProperty]
        public RegisterInputModel Input { get; set; }

        public string? ErrorMessage { get; set; }

        public void OnGet() { }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            // Kiểm tra email đã tồn tại chưa
            var existingUser = _context.Users.FirstOrDefault(u => u.Email == Input.Email);
            if (existingUser != null)
            {
                ErrorMessage = "Email đã tồn tại.";
                return Page();
            }

            // TODO: Nên mã hóa mật khẩu bằng BCrypt hoặc một thư viện khác
            var user = new User
            {
                Email = Input.Email,
                Password = Input.Password,
                Username = Input.FullName,
                Role = UserRole.Customer.ToString()
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync(); // để lấy UserId sau khi thêm

            // Tạo hồ sơ người dùng
            var userProfile = new UserProfile
            {
                UserId = user.Id,
                FullName = Input.FullName,
                Phone = Input.PhoneNumber,
                Address = $"{Input.Ward}, {Input.District}, {Input.Province}",
                AvatarUrl = "" // Có thể để trống hoặc mặc định
            };

            _context.UserProfiles.Add(userProfile);
            await _context.SaveChangesAsync();

            return RedirectToPage("/Login/Index");
        }

        public class RegisterInputModel
        {
            [Required(ErrorMessage = "Vui lòng nhập họ và tên.")]
            public string FullName { get; set; }

            [Required, EmailAddress(ErrorMessage = "Email không hợp lệ.")]
            public string Email { get; set; }

            [Required, DataType(DataType.Password)]
            [StringLength(100, MinimumLength = 6, ErrorMessage = "Mật khẩu tối thiểu 6 ký tự.")]
            public string Password { get; set; }

            [Required, DataType(DataType.Password)]
            [Compare("Password", ErrorMessage = "Mật khẩu xác nhận không khớp.")]
            public string ConfirmPassword { get; set; }

            [Required(ErrorMessage = "Vui lòng nhập số điện thoại.")]
            [Phone(ErrorMessage = "Số điện thoại không hợp lệ.")]
            public string PhoneNumber { get; set; }

            [Required(ErrorMessage = "Vui lòng nhập Tỉnh/Thành phố.")]
            public string Province { get; set; }

            [Required(ErrorMessage = "Vui lòng nhập Quận/Huyện.")]
            public string District { get; set; }

            [Required(ErrorMessage = "Vui lòng nhập Xã/Phường.")]
            public string Ward { get; set; }
        }
    }
}
