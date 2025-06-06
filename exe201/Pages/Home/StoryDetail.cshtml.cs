using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using exe201.Models;

namespace exe201.Pages.Home
{
    public class StoryDetailModel : PageModel
    {
        public Story Story { get; set; }

        [BindProperty(SupportsGet = true)]
        public int Id { get; set; }

        [BindProperty(SupportsGet = true)]
        public int PageNumber { get; set; }

        [BindProperty(SupportsGet = true)]
        public string SelectedCategory { get; set; }

        public void OnGet()
        {
            Story = StoryRepository.GetById(Id);
        }
    }
}
