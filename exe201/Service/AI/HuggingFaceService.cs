using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace exe201.Service.AI
{
    public class CohereService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;

        public CohereService(IHttpClientFactory clientFactory)
        {
            _httpClient = clientFactory.CreateClient("Cohere");
            _apiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY")
                ?? throw new ArgumentNullException("COHERE_API_KEY is not configured.");
        }

        public async Task<string> GetCohereResponseAsync(string prompt)
        {
            var requestBody = new
            {
                model = "command",
                prompt = prompt,
                max_tokens = 100
            };

            try
            {
                _httpClient.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _apiKey);

                var response = await _httpClient.PostAsJsonAsync("generate", requestBody);
                Console.WriteLine($"Cohere API Request: {response.RequestMessage.RequestUri}");

                if (!response.IsSuccessStatusCode)
                {
                    var errorMessage = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Cohere API Error: {response.StatusCode} - {errorMessage}");
                    throw new Exception($"Error from Cohere API: {response.StatusCode} - {errorMessage}");
                }

                var result = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Cohere API Raw Response: {result}");

                var jsonResponse = JsonDocument.Parse(result);
                if (jsonResponse.RootElement.TryGetProperty("generations", out var generations) &&
                    generations.ValueKind == JsonValueKind.Array && generations.GetArrayLength() > 0)
                {
                    if (generations[0].TryGetProperty("text", out var textProperty))
                    {
                        var aiResponseText = textProperty.GetString();
                        Console.WriteLine($"Extracted Text: {aiResponseText}");
                        return aiResponseText?.Trim() ?? "No response text found.";
                    }
                    else
                    {
                        Console.WriteLine("Error: 'generations[0]' does not contain 'text' property.");
                        return "Response 'generations' item does not contain 'text' property.";
                    }
                }
                else
                {
                    Console.WriteLine("Error: Response does not contain 'generations' array or is empty.");
                    return "Response does not contain expected 'generations' array.";
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"CohereService Error: {ex.Message}");
                return $"Lỗi: {ex.Message}";
            }
        }
    }
}