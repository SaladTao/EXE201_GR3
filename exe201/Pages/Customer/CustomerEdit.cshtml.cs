using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using exe201.Models;
using Microsoft.AspNetCore.Http; 
namespace exe201.Pages.Customer
{
    public class CustomerEditModel : PageModel
    {
        private readonly exe201.Models.EcommerceContext _context;

        public CustomerEditModel(exe201.Models.EcommerceContext context)
        {
            _context = context;
        }

        [BindProperty]
        public UserProfile UserProfile { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync()
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToPage("/Login/Index");
            }

            var userProfile = await _context.UserProfiles.FirstOrDefaultAsync(m => m.UserId == userId);
            if (userProfile == null)
            {
               
                UserProfile = new UserProfile { UserId = userId.Value };
            }
            else
            {
                UserProfile = userProfile;
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToPage("/Login/Index");
            }

            UserProfile.UserId = userId.Value;
             
            if (string.IsNullOrWhiteSpace(UserProfile.AvatarUrl))
            {
                UserProfile.AvatarUrl = "/images/default-profile.png";
            }

            var existingProfile = await _context.UserProfiles.FirstOrDefaultAsync(p => p.UserId == userId);

            if (existingProfile == null)
            {
                _context.UserProfiles.Add(UserProfile);
            }
            else
            {
                _context.Entry(existingProfile).CurrentValues.SetValues(UserProfile);
            }

            await _context.SaveChangesAsync();

            return RedirectToPage();
        }

    }
}