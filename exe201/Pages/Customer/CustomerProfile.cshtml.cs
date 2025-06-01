using exe201.Service;
using exe201.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace exe201.Pages.Customer
{
    public class CustomerProfileModel : PageModel
    {
        private readonly ICustomerService _cusService;
        public CustomerProfileModel(ICustomerService cusService)
        {
            _cusService = cusService;
        }

        public CustomerProfileViewModel? Profile { get; set; }
        public async Task<IActionResult> OnGetAsync()
        {
            var userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
            {
                return RedirectToPage("/Login/Index");
            }

            Profile = await _cusService.GetCustomerProfileAsync(userId.Value);

            if (Profile == null)
            {
                return NotFound();
            }

            return Page();
        }

    }
}