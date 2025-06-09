using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Threading.Tasks;
using exe201.Service.AI;
using Microsoft.AspNetCore.Mvc;
using System;

namespace exe201.Pages.Chat
{
    public class ChatModel : PageModel
    {
        private readonly CohereService _cohereService;

        public ChatModel(CohereService cohereService)
        {
            _cohereService = cohereService;
        }

        [BindProperty]
        public string UserInput { get; set; }

        public string AIResponse { get; set; }

        public string ErrorMessage { get; set; }

        public class ChatInput
        {
            public string UserInput { get; set; }
        }

        public async Task<IActionResult> OnPostAsync([FromBody] ChatInput input)
        { 
            if (input != null && !string.IsNullOrEmpty(input.UserInput))
            {
                try
                {
                    var aiResponse = await _cohereService.GetCohereResponseAsync(input.UserInput);
                    if (string.IsNullOrEmpty(aiResponse))
                    {
                        return new JsonResult(new { AIResponse = "Không nhận được phản hồi từ AI." });
                    }

                    return new JsonResult(new { AIResponse = aiResponse });
                }
                catch (Exception ex)
                {
                    return new JsonResult(new { AIResponse = $"Lỗi: {ex.Message}" });
                }
            }
             
            if (!string.IsNullOrEmpty(UserInput))
            {
                try
                {
                    AIResponse = await _cohereService.GetCohereResponseAsync(UserInput);
                    if (string.IsNullOrEmpty(AIResponse))
                    {
                        ErrorMessage = "Không nhận được phản hồi từ AI.";
                    }
                }
                catch (Exception ex)
                {
                    ErrorMessage = "Có lỗi khi lấy phản hồi từ AI: " + ex.Message;
                }
            }

            if (!string.IsNullOrEmpty(AIResponse))
            {
                return Page();
            }

            return Page();
        }
    }
}
