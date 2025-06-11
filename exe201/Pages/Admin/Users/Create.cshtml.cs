using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using exe201.Models;

namespace exe201.Pages.Admin.Users
{
    public class CreateModel : PageModel
    {
        private readonly exe201.Models.EcommerceContext _context;

        public CreateModel(exe201.Models.EcommerceContext context)
        {
            _context = context;
        }

        [BindProperty]
        public User User { get; set; } = default!;
        public SelectList RoleOptions { get; set; } = default!;

        public IActionResult OnGet()
        {
            var userIdStr = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userIdStr))
            {
                return RedirectToPage("/Login/Index");
            }
            User = new User { Profile = new UserProfile() };
            RoleOptions = new SelectList(Enum.GetValues(typeof(UserRole))
                                    .Cast<UserRole>()
                                    .Select(r => r.ToString()), User.Role);
            return Page();
        }

        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            var userIdStr = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userIdStr))
            {
                return RedirectToPage("/Login/Index");
            }
            User.Profile.AvatarUrl = "";
            _context.Users.Add(User);
            await _context.SaveChangesAsync();
            
            return RedirectToPage("./Index");
        }
    }
}
