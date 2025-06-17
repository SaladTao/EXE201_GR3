using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;

namespace exe201.Service.AI
{
    public class GeminiService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;
        private readonly ILogger<GeminiService> _logger;
        private readonly string _systemPrompt;
        private readonly List<string> _conversationHistory;
        private string _lastResponse;

        public GeminiService(HttpClient httpClient, ILogger<GeminiService> logger)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _conversationHistory = new List<string>();
            _lastResponse = string.Empty;

            _apiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY")
                ?? throw new InvalidOperationException("GEMINI_API_KEY chưa được thiết lập trong biến môi trường.");

            _systemPrompt = @"Bạn là một trợ lý AI của một nền tảng thương mại điện tử bán sản phẩm mây tre đan. Nhiệm vụ của bạn là gợi ý sản phẩm cụ thể, phù hợp với nhu cầu người dùng, với câu trả lời ngắn gọn (2-3 câu) và luôn đề xuất ít nhất một sản phẩm từ danh sách có sẵn (giá bằng VND). Sản phẩm bao gồm giỏ mây, đồ nội thất, đồ trang trí, đồ dùng nhà bếp, và túi xách. Nếu câu hỏi chứa 'các sản phẩm bạn vừa liệt kê' hoặc 'sản phẩm vừa đề cập', chỉ gợi ý từ các sản phẩm trong phản hồi trước. Nếu hỏi về sản phẩm không có, thông báo và gợi ý thay thế. Nếu hỏi so sánh, nêu rõ sự khác biệt về giá, chất liệu, công dụng. Nếu hỏi chung chung, gợi ý sản phẩm phổ biến và hỏi thêm. Nếu hỏi ngoài lề, trả lời ngắn gọn và gợi ý sản phẩm. Phân tích đặc điểm cá nhân (giới tính, sở thích, chiều cao) hoặc ngữ cảnh (bếp, tiệc, quà tặng) để gợi ý phù hợp.";
        }

        private const string ApiUrl = "https://generativelanguage.googleapis.com/v1beta/models/gemini-1.5-flash:generateContent";

        public async Task<string> GetResponseFromGemini(string userMessage)
        {
            try
            {
                // Kiểm tra câu hỏi ngắn
                if (string.IsNullOrWhiteSpace(userMessage) || userMessage.Length < 10)
                {
                    userMessage = "Gợi ý sản phẩm mây tre đan phù hợp để trang trí nhà.";
                    _logger.LogWarning("Câu hỏi người dùng quá ngắn, sử dụng câu hỏi mặc định: {UserMessage}", userMessage);
                }

                // Lưu câu hỏi
                _conversationHistory.Add(userMessage);
                if (_conversationHistory.Count > 5)
                {
                    _conversationHistory.RemoveAt(0);
                }

                // Phân tích câu hỏi
                bool refersToLastResponse = Regex.IsMatch(userMessage.ToLower(), @"các sản phẩm bạn vừa liệt kê|sản phẩm vừa đề cập");
                bool isComparison = userMessage.ToLower().Contains("so sánh");
                bool isBudgetQuery = Regex.IsMatch(userMessage.ToLower(), @"dưới\s*(\d+)\.?\d*\s*vnd");
                string productContext = string.Empty;

                // Xử lý câu hỏi theo trường hợp
                if (refersToLastResponse && !string.IsNullOrEmpty(_lastResponse))
                {
                    productContext = _lastResponse;
                }
                else
                {
                    var products = JsonConvert.DeserializeObject<List<dynamic>>(GetProductContext());
                    if (isBudgetQuery)
                    {
                        var match = Regex.Match(userMessage, @"dưới\s*(\d+)\.?\d*\s*VND");
                        if (match.Success && int.TryParse(match.Groups[1].Value, out int budget))
                        {
                            products = products.Where(p => (int)p.Price <= budget).ToList();
                        }
                    }
                    else if (isComparison)
                    {
                        // Lấy các sản phẩm được yêu cầu so sánh
                        var productNames = ExtractProductNames(userMessage);
                        products = products.Where(p => productNames.Any(name => p.Name.ToString().ToLower().Contains(name.ToLower()))).ToList();
                    }
                    else
                    {
                        // Lọc theo loại sản phẩm nếu có từ khóa
                        var keywords = new[] { "túi", "đĩa", "giỏ", "khay", "đèn" };
                        var matchedKeyword = keywords.FirstOrDefault(k => userMessage.ToLower().Contains(k));
                        if (matchedKeyword != null)
                        {
                            products = products.Where(p => p.Name.ToString().ToLower().Contains(matchedKeyword)).ToList();
                        }
                    }
                    productContext = JsonConvert.SerializeObject(products);
                }

                // Tạo lời nhắc
                string fullPrompt = $"{_systemPrompt}\n\nLịch sử câu hỏi: {string.Join("; ", _conversationHistory)}";
                if (!string.IsNullOrEmpty(productContext))
                {
                    fullPrompt += $"\n\nSản phẩm có sẵn: {productContext}";
                }
                fullPrompt += $"\n\nCâu hỏi người dùng: {userMessage}";

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
                        maxOutputTokens = 500
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
                                _lastResponse = candidate.content.parts[0].text?.ToString() ?? string.Empty;
                                return _lastResponse;
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

        private List<string> ExtractProductNames(string userMessage)
        {
            var products = JsonConvert.DeserializeObject<List<dynamic>>(GetProductContext());
            var productNames = products.Select(p => p.Name.ToString()).ToList();
            var matchedNames = new List<string>();
            foreach (var name in productNames)
            {
                if (userMessage.ToLower().Contains(name.ToLower()))
                {
                    matchedNames.Add(name);
                }
            }
            return matchedNames;
        }

        private string GetProductContext()
        {
            var products = new[]
            {
                new { Name = "Giỏ mây nhỏ", Price = 120000, Description = "Giỏ mây đan tay, thiết kế đơn giản, lý tưởng cho bàn trang điểm hoặc kệ sách.", Category = "Trang trí" },
                new { Name = "Đĩa mây trang trí", Price = 339000, Description = "Đĩa mây phẳng, tối giản, dùng làm khay đựng nến, đồ trang trí, hoặc đồ khô trong bếp.", Category = "Trang trí" },
                new { Name = "Hộp mây mini", Price = 339000, Description = "Hộp mây có nắp, thiết kế tinh tế, phù hợp để lưu trữ phụ kiện nhỏ.", Category = "Lưu trữ" },
                new { Name = "Đĩa sứ viền mây tre đan tay trang trí tiệc", Price = 135000, Description = "Đĩa sứ với viền mây đan tay, dùng để trình bày món ăn trong bếp hoặc trang trí bàn tiệc.", Category = "Trang trí" },
                new { Name = "Khay mây khảm trai bầu dục có tay cầm", Price = 190000, Description = "Khay mây khảm trai cao cấp, đa năng, phù hợp để đựng đồ hoặc trang trí.", Category = "Trang trí" },
                new { Name = "Khay mây tròn có tay cầm", Price = 3500000, Description = "Khay mây tròn ECOLOOM, dùng để đựng đồ, trang trí tiệc hoặc chụp ảnh sản phẩm.", Category = "Trang trí" },
                new { Name = "Giỏ mây hình chữ nhật đáy vát", Price = 680000, Description = "Giỏ mây chữ nhật đáy vát, dùng để đựng hoa quả, bánh kẹo, phong cách vintage.", Category = "Lưu trữ" },
                new { Name = "Xích đu mây decor", Price = 540000, Description = "Xích đu mây cao cấp, phù hợp để trang trí sân vườn, ban công hoặc phòng khách.", Category = "Nội thất" },
                new { Name = "Đèn lồng tre đan thủ công", Price = 540000, Description = "Đèn lồng tre đan tay, dùng trang trí nhà hàng, quán cà phê hoặc nhà cửa.", Category = "Trang trí" },
                new { Name = "Đèn tre lồng tròn", Price = 540000, Description = "Đèn tre lồng tròn thủ công, tạo ánh sáng ấm áp cho không gian sống.", Category = "Trang trí" },
                new { Name = "Đèn trụ dài khung tre", Price = 439000, Description = "Đèn trụ dài khung tre với lớp vải thả, lý tưởng để trang trí phòng khách hoặc nhà hàng.", Category = "Trang trí" },
                new { Name = "Túi xách bện mây cho nữ", Price = 160000, Description = "Túi xách mây đan tay, thiết kế thời trang, phù hợp cho nữ.", Category = "Túi xách" },
                new { Name = "Túi mây tre đan tròn sáng quai da", Price = 510000, Description = "Túi mây tròn với quai da, phong cách hiện đại, phù hợp để đi chơi hoặc công việc.", Category = "Túi xách" },
                new { Name = "Túi mây tre đan tròn họa tiết viền hoa", Price = 510000, Description = "Túi mây tròn với họa tiết viền hoa, thời trang và nổi bật.", Category = "Túi xách" },
                new { Name = "Túi xách mây tre đan hình chữ nhật", Price = 18000, Description = "Túi xách mây hình chữ nhật với quai sắc, phong cách cổ điển, giá phải chăng.", Category = "Túi xách" },
                new { Name = "Túi xách mây tre hình tròn", Price = 900000, Description = "Túi xách mây hình tròn, thiết kế tinh tế, phù hợp cho các dịp đặc biệt.", Category = "Túi xách" },
                new { Name = "Khay mây lượn sóng", Price = 900000, Description = "Khay mây lượn sóng, làm từ mây tự nhiên chống mối mọt, dùng để đựng hoa quả, bánh kẹo hoặc đồ khô.", Category = "Lưu trữ" }
            };
            return JsonConvert.SerializeObject(products);
        }
    }
}