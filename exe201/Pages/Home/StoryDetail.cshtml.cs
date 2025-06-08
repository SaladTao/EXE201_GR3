using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using exe201.Models;
using System.Text.RegularExpressions;

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

        public string GetFormattedDescription(string rawDescription)
        {
            if (string.IsNullOrEmpty(rawDescription))
                return string.Empty;

            string html = Regex.Replace(
    rawDescription,
    @"(/images/[\w\-/]+\.(png|jpg|jpeg|gif))",
    "<br /><img src=\"$1\" alt=\"Hình ảnh\" style=\"max-width:100%; margin: 10px auto; display: block;\" /><br />"
);


            html = html.Replace("\r\n", "<br />")
                       .Replace("\n", "<br />")
                       .Replace("\r", "<br />");

            return html;
        }
    }
}
