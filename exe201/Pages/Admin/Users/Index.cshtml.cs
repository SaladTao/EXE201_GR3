using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using exe201.Models;

namespace exe201.Pages.Admin.Users
{
    public class IndexModel : PageModel
    {
        private readonly exe201.Models.EcommerceContext _context;

        public IndexModel(exe201.Models.EcommerceContext context)
        {
            _context = context;
        }

        public IList<User> User { get;set; } = default!;

        public async Task OnGetAsync()
        {
            User = await _context.Users.Include(u => u.Profile).ToListAsync();
        }
    }
}
