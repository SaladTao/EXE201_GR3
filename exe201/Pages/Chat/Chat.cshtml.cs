using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Threading.Tasks;
using exe201.Service.AI;
using Microsoft.AspNetCore.Mvc;
using System;

namespace exe201.Pages.Chat
{
    //public class ChatModel : PageModel
    //{
    //    private readonly CohereService _cohereService;

    //    public ChatModel(CohereService cohereService)
    //    {
    //        _cohereService = cohereService;
    //    }

    //    [BindProperty]
    //    public string UserInput { get; set; }

    //    public string AIResponse { get; set; }

    //    public string ErrorMessage { get; set; }

    //    public class ChatInput
    //    {
    //        public string UserInput { get; set; }
    //    }

    //    public async Task<IActionResult> OnPostAsync([FromBody] ChatInput input)
    //    { 
    //        if (input != null && !string.IsNullOrEmpty(input.UserInput))
    //        {
    //            try
    //            {
    //                var aiResponse = await _cohereService.GetCohereResponseAsync(input.UserInput);
    //                if (string.IsNullOrEmpty(aiResponse))
    //                {
    //                    return new JsonResult(new { AIResponse = "Không nhận được phản hồi từ AI." });
    //                }

    //                return new JsonResult(new { AIResponse = aiResponse });
    //            }
    //            catch (Exception ex)
    //            {
    //                return new JsonResult(new { AIResponse = $"Lỗi: {ex.Message}" });
    //            }
    //        }

    //        if (!string.IsNullOrEmpty(UserInput))
    //        {
    //            try
    //            {
    //                AIResponse = await _cohereService.GetCohereResponseAsync(UserInput);
    //                if (string.IsNullOrEmpty(AIResponse))
    //                {
    //                    ErrorMessage = "Không nhận được phản hồi từ AI.";
    //                }
    //            }
    //            catch (Exception ex)
    //            {
    //                ErrorMessage = "Có lỗi khi lấy phản hồi từ AI: " + ex.Message;
    //            }
    //        }

    //        if (!string.IsNullOrEmpty(AIResponse))
    //        {
    //            return Page();
    //        }

    //        return Page();
    //    }
    //}

    public class ChatModel : PageModel
    {
        private readonly GeminiService _geminiService;
        private readonly ILogger<ChatModel> _logger;

        [BindProperty]
        public string UserInput { get; set; }

        public string AIResponse { get; set; }

        public ChatModel(GeminiService geminiService, ILogger<ChatModel> logger)
        {
            _geminiService = geminiService;
            _logger = logger;
        }
        public void OnGet()
        {
            // Chế độ xem ban đầu (khi chưa có câu hỏi)
            AIResponse = null;
        }

        public async Task<IActionResult> OnPostAsync([FromBody] ChatRequest request)
        {
            _logger.LogInformation("Received POST request with UserInput: {UserInput}", request?.UserInput);

            if (string.IsNullOrEmpty(request?.UserInput))
            {
                _logger.LogWarning("UserInput is empty.");

                // Trả về thông báo lỗi nếu userInput không có nội dung
                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    var badRequestResponse = new { aiResponse = "Vui lòng nhập nội dung." };
                    return new JsonResult(badRequestResponse) { StatusCode = 400 };  // Trả về mã lỗi 400
                }

                return Page();  // Trả về trang nếu không phải AJAX
            }

            try
            {
                // Gọi API Gemini với UserInput
                AIResponse = await _geminiService.GetResponseFromGemini(request.UserInput);
                _logger.LogInformation("Gemini AI Response: {AIResponse}", AIResponse);

                // Trả về JSON cho AJAX nếu yêu cầu từ AJAX
                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    var jsonResponse = new { aiResponse = AIResponse };
                    return new JsonResult(jsonResponse);  // Trả về phản hồi JSON
                }

                return Page();  // Trả về trang nếu không phải AJAX
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calling Gemini API.");
                var errorResponse = $"Lỗi khi gọi Gemini API: {ex.Message}";

                // Trả về lỗi dưới dạng JSON cho AJAX nếu có lỗi
                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    var jsonErrorResponse = new { aiResponse = errorResponse };
                    return new JsonResult(jsonErrorResponse) { StatusCode = 500 }; // Lỗi 500
                }

                AIResponse = errorResponse;  // Hiển thị lỗi trên trang nếu không phải AJAX
                return Page();
            }
        }
    }

    public class ChatRequest
    {
        public string UserInput { get; set; }
    }
}
