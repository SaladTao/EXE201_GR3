using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using System.Linq;
using exe201.Models; 
namespace exe201.Pages.Home
{
    public class StoryModel : PageModel
    {
        public List<Story> Stories { get; set; }

        [BindProperty(SupportsGet = true, Name = "selectedCategory")]
        public string SelectedCategory { get; set; }
        public List<string> Categories { get; set; }

        [BindProperty(SupportsGet = true)]
        public int PageNumber { get; set; } = 1;
        public int TotalPages { get; set; }
        public int PageSize = 9;

        public void OnGet()
        {
            var allStories = StoryRepository.GetAll();

            Categories = allStories
                .Select(s => s.Category)
                .Distinct()
                .OrderBy(c => c)
                .ToList();

            var filteredStories = string.IsNullOrEmpty(SelectedCategory)
                ? allStories.OrderBy(s => s.Id).ToList()
                : allStories.Where(s => s.Category == SelectedCategory)
                            .OrderBy(s => s.Id)
                            .ToList();

            TotalPages = (int)Math.Ceiling(filteredStories.Count / (double)PageSize);

            var pagedStories = filteredStories
                .Skip((PageNumber - 1) * PageSize)
                .Take(PageSize)
                .ToList();

            int displayStart = (PageNumber - 1) * PageSize + 1;
            for (int i = 0; i < pagedStories.Count; i++)
            {
                pagedStories[i].Id = displayStart + i;
            }

            Stories = pagedStories;
        }
    }
}
