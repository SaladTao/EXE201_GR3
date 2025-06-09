using Newtonsoft.Json;
using System.Text;
using System;
using System.Net.Http;
using Microsoft.Extensions.Logging;

namespace exe201.Service.AI
{
    public class GeminiService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;
        private readonly ILogger<GeminiService> _logger;

        public GeminiService(HttpClient httpClient, ILogger<GeminiService> logger)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            // Đọc API key từ biến môi trường
            _apiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY")
                ?? throw new InvalidOperationException("GEMINI_API_KEY not set in environment variables.");
        }

        private const string ApiUrl = "https://generativelanguage.googleapis.com/v1beta/models/gemini-1.5-flash:generateContent";

        public async Task<string> GetResponseFromGemini(string userMessage)
        {
            try
            {
                // Cấu trúc yêu cầu cho Gemini API
                var requestBody = new
                {
                    contents = new[]
                    {
                        new
                        {
                            parts = new[]
                            {
                                new { text = userMessage }
                            }
                        }
                    },
                    generationConfig = new
                    {
                        temperature = 0.7,
                        maxOutputTokens = 100
                    }
                };

                var jsonContent = JsonConvert.SerializeObject(requestBody);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                // Tạo HttpRequestMessage và thêm API key vào query string
                var requestMessage = new HttpRequestMessage(HttpMethod.Post, $"{ApiUrl}?key={_apiKey}")
                {
                    Content = content
                };

                // Gửi yêu cầu HTTP
                var response = await _httpClient.SendAsync(requestMessage);
                if (response.IsSuccessStatusCode)
                {
                    var jsonResponse = await response.Content.ReadAsStringAsync();
                    _logger.LogInformation("Gemini API response: {JsonResponse}", jsonResponse); // In phản hồi JSON

                    try
                    {
                        dynamic result = JsonConvert.DeserializeObject(jsonResponse);

                        // Kiểm tra cấu trúc JSON
                        if (result?.candidates != null && result.candidates.Count > 0)
                        {
                            var candidate = result.candidates[0];
                            if (candidate?.content?.parts != null && candidate.content.parts.Count > 0)
                            {
                                return candidate.content.parts[0].text?.ToString() ?? "No response text found in candidate parts.";
                            }
                        }
                        return "No response text found in candidates.";
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Failed to parse Gemini API response: {JsonResponse}", jsonResponse);
                        return $"Error parsing response: {ex.Message}";
                    }
                }
                else
                {
                    var errorResponse = await response.Content.ReadAsStringAsync();
                    _logger.LogError("Gemini API request failed. Status: {StatusCode}, Details: {ErrorResponse}", response.StatusCode, errorResponse);
                    return $"Error: {response.StatusCode}, Details: {errorResponse}";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while calling Gemini API.");
                return $"Error: {ex.Message}";
            }
        }
    }
}