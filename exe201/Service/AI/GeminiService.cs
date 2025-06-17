using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace exe201.Service.AI
{
    public class GeminiService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;
        private readonly ILogger<GeminiService> _logger;
        private readonly string _systemPrompt;

        public GeminiService(HttpClient httpClient, ILogger<GeminiService> logger)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            _apiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY")
                ?? throw new InvalidOperationException("GEMINI_API_KEY chưa được thiết lập trong biến môi trường.");
             
            _systemPrompt = @"Bạn là một trợ lý AI của một nền tảng thương mại điện tử bán sản phẩm mây tre đan. Nhiệm vụ của bạn là gợi ý sản phẩm cụ thể, phù hợp với nhu cầu người dùng, với câu trả lời ngắn gọn (2-3 câu) và luôn đề xuất ít nhất một sản phẩm từ danh sách có sẵn. Sản phẩm bao gồm giỏ mây, đồ nội thất (ghế, bàn, kệ), đồ trang trí (đèn, khay, đĩa), và đồ dùng nhà bếp (tấm lót, đế lót ly). Nếu câu hỏi không rõ, hãy gợi ý sản phẩm phổ biến như giỏ mây hoặc đĩa mây và hỏi thêm để làm rõ.";
        }

        private const string ApiUrl = "https://generativelanguage.googleapis.com/v1beta/models/gemini-1.5-flash:generateContent";

        public async Task<string> GetResponseFromGemini(string userMessage)
        {
            try
            { 
                if (string.IsNullOrWhiteSpace(userMessage) || userMessage.Length < 10)
                {
                    userMessage = "Gợi ý sản phẩm mây tre đan phù hợp để trang trí nhà.";
                    _logger.LogWarning("Câu hỏi người dùng quá ngắn, sử dụng câu hỏi mặc định: {UserMessage}", userMessage);
                }
                 
                var fullPrompt = $"{_systemPrompt}\n\nCâu hỏi người dùng: {userMessage}";

                var productContext = GetProductContext();
                if (!string.IsNullOrEmpty(productContext))
                {
                    fullPrompt += $"\n\nSản phẩm có sẵn: {productContext}";
                }

                var requestBody = new
                {
                    contents = new[]
                    {
                        new
                        {
                            parts = new[]
                            {
                                new { text = fullPrompt }
                            }
                        }
                    },
                    generationConfig = new
                    {
                        temperature = 0.7,
                        maxOutputTokens = 800  
                    }
                };

                var jsonContent = JsonConvert.SerializeObject(requestBody);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                var requestMessage = new HttpRequestMessage(HttpMethod.Post, $"{ApiUrl}?key={_apiKey}")
                {
                    Content = content
                };

                var response = await _httpClient.SendAsync(requestMessage);
                if (response.IsSuccessStatusCode)
                {
                    var jsonResponse = await response.Content.ReadAsStringAsync();
                    _logger.LogInformation("Phản hồi từ Gemini API: {JsonResponse}", jsonResponse);

                    try
                    {
                        dynamic result = JsonConvert.DeserializeObject(jsonResponse);

                        if (result?.candidates != null && result.candidates.Count > 0)
                        {
                            var candidate = result.candidates[0];
                            if (candidate?.content?.parts != null && candidate.content.parts.Count > 0)
                            {
                                return candidate.content.parts[0].text?.ToString() ?? "Không tìm thấy văn bản phản hồi.";
                            }
                        }
                        return "Không tìm thấy văn bản phản hồi trong candidates.";
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Lỗi khi phân tích phản hồi từ Gemini API: {JsonResponse}", jsonResponse);
                        return $"Lỗi phân tích phản hồi: {ex.Message}";
                    }
                }
                else
                {
                    var errorResponse = await response.Content.ReadAsStringAsync();
                    _logger.LogError("Yêu cầu Gemini API thất bại. Trạng thái: {StatusCode}, Chi tiết: {ErrorResponse}", response.StatusCode, errorResponse);
                    return $"Lỗi: {response.StatusCode}, Chi tiết: {errorResponse}";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi gọi Gemini API.");
                return $"Lỗi: {ex.Message}";
            }
        }


        private string GetProductContext()
        {
            var products = new[]
            {
                new { Name = "Giỏ mây nhỏ", Price = 120000, Description = "Giỏ mây đan tay, thiết kế đơn giản, lý tưởng cho bàn trang điểm hoặc kệ sách." },
                new { Name = "Đĩa mây trang trí", Price = 339000, Description = "Đĩa mây phẳng, tối giản, dùng làm khay đựng nến hoặc đồ trang trí." },
                new { Name = "Hộp mây mini", Price = 339000, Description = "Hộp mây có nắp, thiết kế tinh tế, phù hợp để lưu trữ phụ kiện nhỏ." },
                new { Name = "Đĩa sứ viền mây tre đan tay trang trí tiệc", Price = 135000, Description = "Đĩa sứ với viền mây đan tay, lý tưởng để trang trí bàn tiệc hoặc phòng khách." },
                new { Name = "Khay mây khảm trai bầu dục có tay cầm", Price = 190000, Description = "Khay mây khảm trai cao cấp, đa năng, phù hợp để đựng đồ hoặc trang trí." },
                new { Name = "Khay mây tròn có tay cầm", Price = 3500000, Description = "Khay mây tròn ECOLOOM, dùng để đựng đồ, trang trí tiệc hoặc chụp ảnh sản phẩm." },
                new { Name = "Giỏ mây hình chữ nhật đáy vát", Price = 680000, Description = "Giỏ mây chữ nhật đáy vát, dùng để đựng hoa quả, bánh kẹo, phong cách vintage." },
                new { Name = "Xích đu mây decor", Price = 540000, Description = "Xích đu mây cao cấp, phù hợp để trang trí sân vườn, ban công hoặc phòng khách." },
                new { Name = "Đèn lồng tre đan thủ công", Price = 540000, Description = "Đèn lồng tre đan tay, dùng trang trí nhà hàng, quán cà phê hoặc nhà cửa." },
                new { Name = "Đèn tre lồng tròn", Price = 540000, Description = "Đèn tre lồng tròn thủ công, tạo ánh sáng ấm áp cho không gian sống." },
                new { Name = "Đèn trụ dài khung tre", Price = 439000, Description = "Đèn trụ dài khung tre với lớp vải thả, lý tưởng để trang trí phòng khách hoặc nhà hàng." },
                new { Name = "Túi xách bện mây cho nữ", Price = 160000, Description = "Túi xách mây đan tay, thiết kế thời trang, phù hợp cho nữ." },
                new { Name = "Túi mây tre đan tròn sáng quai da", Price = 510000, Description = "Túi mây tròn với quai da, phong cách hiện đại, phù hợp để đi chơi hoặc công việc." },
                new { Name = "Túi mây tre đan tròn họa tiết viền hoa", Price = 510000, Description = "Túi mây tròn với họa tiết viền hoa, thời trang và nổi bật." },
                new { Name = "Túi xách mây tre đan hình chữ nhật", Price = 18000, Description = "Túi xách mây hình chữ nhật với quai sắc, phong cách cổ điển, giá phải chăng." },
                new { Name = "Túi xách mây tre hình tròn", Price = 900000, Description = "Túi xách mây hình tròn, thiết kế tinh tế, phù hợp cho các dịp đặc biệt." },
                new { Name = "Khay mây lượn sóng", Price = 900000, Description = "Khay mây lượn sóng, làm từ mây tự nhiên chống mối mọt, dùng để đựng hoa quả, bánh kẹo hoặc đồ khô." }
            };
            return JsonConvert.SerializeObject(products);
        }
    }
}