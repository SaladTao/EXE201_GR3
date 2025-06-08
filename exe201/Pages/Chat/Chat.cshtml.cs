using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;
using System.Text;

namespace exe201.Pages.Chat
{
    public class ChatModel : PageModel
    {
        [BindProperty]
        public string UserMessage { get; set; }

        public string AIResponse { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!string.IsNullOrWhiteSpace(UserMessage))
            {
                AIResponse = await GetAIResponse(UserMessage);
            }
            return Page();
        }

        private async Task<string> GetAIResponse(string message)
        {
            var apiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY");

            using var client = new HttpClient();
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");

            var requestBody = new
            {
                model = "gpt-3.5-turbo",
                messages = new[] {
                new { role = "user", content = message }
            }
            };

            var content = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");

            var response = await client.PostAsync("https://api.openai.com/v1/chat/completions", content);
            var responseString = await response.Content.ReadAsStringAsync();

            using JsonDocument doc = JsonDocument.Parse(responseString);
            var aiMessage = doc.RootElement
                .GetProperty("choices")[0]
                .GetProperty("message")
                .GetProperty("content")
                .GetString();

            return aiMessage;
        }
    }
}
