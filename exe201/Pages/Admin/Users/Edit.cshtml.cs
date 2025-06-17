using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using exe201.Models;

namespace exe201.Pages.Admin.Users
{
    public class EditModel : PageModel
    {
        private readonly exe201.Models.EcommerceContext _context;

        public EditModel(exe201.Models.EcommerceContext context)
        {
            _context = context;
        }

        [BindProperty]
        public User User { get; set; } = default!;
        public SelectList RoleOptions { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            var userIdStr = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userIdStr))
            {
                return RedirectToPage("/Login/Index");
            }
            if (id == null)
            {
                return NotFound();
            }

            var user =  await _context.Users.Include(u => u.Profile).FirstOrDefaultAsync(m => m.Id == id);
            if (user == null)
            {
                return NotFound();
            }
            User = user;
            RoleOptions = new SelectList(Enum.GetValues(typeof(UserRole))
                                   .Cast<UserRole>()
                                   .Select(r => r.ToString()), User.Role);
            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            var userIdStr = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userIdStr))
            {
                return RedirectToPage("/Login/Index");
            }
            // Lấy user gốc từ DB kèm theo profile để update
            var userToUpdate = await _context.Users
                                .Include(u => u.Profile)
                                .FirstOrDefaultAsync(u => u.Id == User.Id);

            if (userToUpdate == null)
                return NotFound();

            // Cập nhật User fields
            userToUpdate.Username = User.Username;
            userToUpdate.Email = User.Email;
            userToUpdate.Role = User.Role;
            userToUpdate.CreatedAt = User.CreatedAt;

            // Nếu profile null thì tạo mới
            if (userToUpdate.Profile == null)
            {
                userToUpdate.Profile = new UserProfile();
            }

            // Cập nhật các trường UserProfile từ model post lên
            userToUpdate.Profile.FullName = User.Profile?.FullName;
            userToUpdate.Profile.Address = User.Profile?.Address;
            userToUpdate.Profile.Phone = User.Profile?.Phone;
            //userToUpdate.Profile.AvatarUrl = User.Profile?.AvatarUrl;

            try
            {
                _context.Update(userToUpdate);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(User.Id))
                    return NotFound();
                else
                    throw;
            }

            return RedirectToPage("./Index");
        }


        private bool UserExists(int id)
        {
            return _context.Users.Any(e => e.Id == id);
        }
    }
}
