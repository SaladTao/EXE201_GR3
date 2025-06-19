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
    // Gia dụng
    new { Name = "Đĩa sứ viền mây tre đan tay trang trí tiệc", Price = 120000, Description = "Đĩa sứ viền mây tre đan là một lựa chọn phổ biến cho sản phẩm đồ dùng bếp hiện nay. Sự hòa quyện tinh tế giữa sứ cao cấp và mây tre tự nhiên không chỉ đem lại độ bền vượt trội và khả năng chịu nhiệt tốt, mà còn tôn lên vẻ đẹp thẩm mỹ, đồng thời bảo vệ môi trường. Cùng ECOLOOM khám phá thêm về những đặc trưng và lợi ích nổi bật của sản phẩm này, để bạn có thể sử dụng và trang trí không gian bếp của mình một cách tinh tế và hoàn hảo nhất nhé! Giới thiệu về sản phẩm đĩa sứ viền mây tre đan tay Đĩa sứ viền mây tre đan thủ công của ECOLOOM là một tác phẩm sứ thượng hạng được tạo ra bởi những nghệ nhân địa phương tại Việt Nam. Điểm đặc biệt của sản phẩm nằm ở việc sử dụng kỹ thuật đan mây tre tinh tế để trang trí, mang đến một vẻ đẹp độc đáo và phong cách tự nhiên. Đĩa sứ viền mây tre đan thủ công có kích thước nhỏ gọn, hoàn hảo cho các bữa ăn gia đình hoặc tiệc nhỏ. Sản phẩm đa dạng về hình dáng, từ vuông, tròn cho đến trái tim, giúp bạn dễ dàng lựa chọn một sản phẩm phù hợp với nhu cầu sử dụng của mình. Màu sắc của đĩa sứ viền mây tre đan thủ công cũng phong phú và đa dạng, từ gam màu trắng tinh khôi đến các tông màu pastel nhẹ nhàng và tươi sáng. Những sắc màu tinh tế này sẽ làm nổi bật bữa ăn của bạn, tạo thêm niềm vui và phấn khích khi thưởng thức món ăn. Sản phẩm đĩa sứ viền mây tre đan thủ công không chỉ là vật dụng sử dụng trong bữa ăn, mà còn là một tác phẩm trang trí tuyệt vời cho bộ sưu tập đồ dùng nhà bếp của bạn. Hơn nữa, sản phẩm này rất dễ dàng sử dụng và bảo quản. Sau khi sử dụng, bạn chỉ cần rửa sạch đĩa bằng nước ấm kết hợp với một ít chất tẩy rửa nhẹ, sau đó lau khô hoặc treo phơi. Để bảo vệ bề mặt sản phẩm, tránh sử dụng các đồ dùng cứng hoặc chất tẩy rửa mạnh, để tránh gây trầy xước hoặc làm hỏng sản phẩm. Những ưu điểm và đặc điểm nổi bật đĩa sứ viền mây tre đan tay 1. Độ bền cao Đĩa sứ viền mây tre đan thủ công cũng thể hiện khả năng chịu nhiệt tốt, đảm bảo sản phẩm không bị biến dạng hay hỏng hóc khi tiếp xúc với nhiệt độ cao. Khả năng chịu nhiệt cao cũng là một ưu điểm quan trọng của đĩa sứ viền mây tre đan tay từ ECOLOOM, mang lại sự yên tâm khi sử dụng trong việc nấu nướng và làm việc trong không gian bếp. Được sử dụng để chưng cất thực phẩm, đun nước hay nấu lẩu an toàn, sản phẩm này đáp ứng mọi yêu cầu của bạn. Không chỉ thế, đĩa sứ viền mây tre đan thủ công cũng dễ dàng làm sạch và mang tính thẩm mỹ cao. Với nguyên liệu tự nhiên, không chứa hóa chất độc hại, sản phẩm đảm bảo an toàn cho sức khỏe của bạn. Những đường viền mây tre tinh tế trên sản phẩm càng tôn lên vẻ đẹp tự nhiên và tinh tế, mang đến cho sản phẩm sự độc đáo và thu hút. Đáng chú ý, đĩa sứ viền mây tre đan thủ công từ ECOLOOM còn hướng đến tính thân thiện với môi trường. Sử dụng sản phẩm này giúp giảm thiểu việc sử dụng đồ nhựa và giảm lượng rác thải phát sinh trong quá trình sử dụng đồ dùng nhà bếp. 2. Độ chịu nhiệt Đĩa sứ viền mây tre đan tay từ ECOLOOM được sản xuất từ sứ cao cấp và mây tre tự nhiên, đem đến khả năng chịu nhiệt vượt trội so với các sản phẩm đồ dùng bếp thông thường. Với khả năng chịu nhiệt lên đến 1200 độ C, sản phẩm này cho phép bạn sử dụng trong các điều kiện nhiệt độ cao như khi đặt trực tiếp lên bếp gas hoặc bếp từ một cách an toàn và thuận tiện. Hơn nữa, việc sử dụng trong điều kiện nhiệt độ cao không làm ảnh hưởng đến chất lượng và độ bền của sản phẩm, biến đĩa sứ viền mây tre đan tay ECOLOOM thành sự lựa chọn hoàn hảo cho những người đam mê nấu ăn. 3. Dễ dàng vệ sinh Để sử dụng sản phẩm lâu bền và duy trì chất lượng, cần quan tâm đến cách bảo quản. Trong quá trình sử dụng, hạn chế sử dụng dao hoặc các công cụ sắc nhọn để cắt và chế biến thực phẩm trực tiếp trên đĩa sứ, để tránh làm trầy xước bề mặt sản phẩm. Sau khi sử dụng, cần rửa sạch sản phẩm và lau khô trước khi đặt vào tủ đựng. Khi lau khô, nên sử dụng khăn mềm hoặc giấy báo, tránh dùng khăn ướt. Để bảo quản tốt hơn, hạn chế tiếp xúc trực tiếp với ánh nắng mặt trời, để tránh tình trạng phai màu. Với những lưu ý như vậy, bạn sẽ có thể sử dụng sản phẩm đĩa sứ viền mây tre đan tay từ ECOLOOM trong thời gian dài và bảo đảm độ bền và chất lượng của sản phẩm. 4. Thân thiện với môi trường Được tạo ra từ các thành phần tự nhiên, đĩa sứ viền mây tre đan tay của ECOLOOM mang tính thân thiện với môi trường. Khác biệt với nhiều sản phẩm đồ dùng bếp khác được chế tạo từ vật liệu nhân tạo, độc hại và khó phân hủy, sản phẩm của ECOLOOM lại được tạo nên từ các nguyên liệu thân thiện với môi trường và có thể được tái chế sau khi sử dụng. Điều này có ý nghĩa quan trọng trong việc giảm thiểu tác động tiêu cực lên môi trường và góp phần bảo vệ tài nguyên thiên nhiên của hành tinh chúng ta. Nếu bạn là người quan tâm đến việc bảo vệ môi trường và ưa chuộng sử dụng các sản phẩm thân thiện với môi trường, thì đĩa sứ viền mây tre đan tay của ECOLOOM sẽ là lựa chọn hoàn hảo dành cho bạn. 5. Tính thẩm mỹ cao Sản phẩm này mang thiết kế đơn giản tuy nhiên vô cùng tinh tế và sang trọng, phù hợp với đa dạng các loại không gian và phong cách trang trí khác nhau. Bề mặt sáng bóng cùng viền mây tre đan tay tạo nên một sự hài hòa đẹp mắt, tinh tế và thu hút sự chú ý của mọi người trong mọi tình huống sử dụng. Hơn nữa, đĩa sứ viền mây tre đan tay từ ECOLOOM được bổ sung bởi màu sắc tự nhiên, tạo nên một không gian bếp ấm cúng và thân thiện. Sản phẩm này được đánh giá cao về khả năng thẩm mỹ và chính vì thế, nó đã trở thành một trong những món đồ dùng bếp được ưa chuộng và yêu thích nhất trên thị trường hiện nay.", Category = "Gia dụng" },
    new { Name = "Khay mây khảm trai bầu dục có tay cầm", Price = 339000, Description = "Khay mây khảm trai bầu dục có tay cầm đựng đồ đa năng, cao cấp, decor – OTB3 – Sản phẩm được làm hoàn toàn thủ công bởi những người thợ có tay nghề cao nên chất lượng sản phẩm luôn ở mức tốt nhất – Vẻ đẹp mộc mạc, tự nhiên, thân thiện với môi trường mang đến độ bền đẹp theo thời gian, dễ dàng vệ sinh, tiết kiệm thời gian ______ ❁ THÔNG TIN SẢN PHẨM: – Chức năng: Đựng đồ ăn, bánh kẹo, decor, chụp ảnh, đồ dùng nhà bếp, đựng đồ khô, mứt, ô mai, hạt các loại … – Thương hiệu: ECOLOOM – Kích thước: 27×20,5x4cm 31×22,5x4cm – Xuất xứ: Việt Nam – Chất liệu: Mây khảm trai – Màu sắc: Tự nhiên – Sản phẩm của ECOLOOM được chọn lựa kỹ trước khi đến tay khách hàng ❁ CÔNG NĂNG: – Khay mây khảm trai ECOLOOM là lựa chọn dành cho những khu vực khí hậu ẩm thấp, không thể sử dụng được đồ dùng mây tre đan tự nhiên. – Khay mây khảm trai decor ECOLOOM dùng trang trí nhà cửa, đựng hoa quả, bánh kẹo rất phù hợp. – Lưu ý sử dụng + Chất liệu dù đã qua xử lý cũng sẽ dễ bị ẩm mốc khi điều kiện bảo quản không tốt. + Bảo quản nơi thoáng mát, khô ráo sau khi sử dụng. ______ ❁ Bảo hành & Dịch vụ – Bảo hành 1 đổi 1 miễn phí trong vòng 7 ngày nếu sản phẩm có lỗi kĩ thuật – Bảo hành chất lượng sản phẩm 01 tháng – Một số trường hợp ECOLOOM bảo hành: + Sản phẩm bị hư hỏng/ bể vỡ trong quá trình vận chuyển + Sản phẩm bị lỗi do lỗi từ nhà sản xuất + Sản phẩm giao đến không đúng, không đủ số lượng theo đơn hàng đã đặt + Những lỗi khác do kỹ thuật, chất liệu của sản phẩm + Chăm sóc, hỗ trợ & tư vấn trọn đời Mẹo khi mua hàng: + Áp dụng đúng mã voucher để được ưu đãi tốt nhất + Khi nhận hàng, khách hàng nên quay video lúc mở sản phẩm để đảm bảo quyền lợi khi có sự cố phải đổi trả sản phẩm + Hãy để lại vài lời đánh giá chân thành về chất lượng sản phẩm/dịch vụ để góp phần ECOLOOM phát triển tốt sản phẩm/ dịch vụ và nhận được thêm nhiều ƯU ĐÃI. #khay #gio #ro #may #khaytrangtri #rotrangtri #giotrangkhi #diadan #giodan #rodan #khaydungdo #giodungdo #rodungdo #maytredan #maytrexuatkhau #phukien #dodung #danang #decor #trangtri #thanthien #moitruong #caocap #chatluongcao #xuatkhau #thucong #mynghe #handmade #handicraft #phukienchupanh", Category = "Gia dụng" },
    new { Name = "Khay mây tròn có tay cầm", Price = 339000, Description = "Khay mây tròn có tay cầm ECOLOOM đựng đồ, trang trí tiệc, decor chụp ảnh sản phẩm ✔ Bộ khay đựng đồ tiện dụng bằng mây đan được làm từ làng nghề thủ công ✔ Nguyên liệu được làm hoàn toàn từ thiên nhiên đã qua xử lý chống mối mọt, hoàn toàn thân thiện với môi trường ✔ Sản phẩm tiện lợi nhiều chức năng, dùng để đựng rau, củ, quả, thực phẩm khô hoặc các loại văn phòng phẩm, mỹ phẩm… ✔ Nên sử dụng thước kẻ để đo, đừng tưởng tượng kích thước sản phẩm. THÔNG TIN SẢN PHẨM: – Sản phẩm được làm thủ công từ những bàn tay của các nghệ nhân lành nghề của làng nghề thủ công Việt Nam – Chất liệu mây tre đan tự nhiên – Sản phẩm thân thiện với môi trường, an toàn cho người sử dụng. * Thích hợp cho: – Dùng làm quà tặng: Cho công ty, gia đình, Cá nhân, nhà hàng, khách sạn – Đựng bánh, kẹo, mứt, trà, trái cây – Trang trí, Decor, sáng tạo cho nhiều mục đích khác nhau – Chụp hình sản phẩm, trang trí phong cách mộc mạc * CÁCH BẢO QUẢN SẢN PHẨM : – Giữ sản phẩm nơi thoáng mát, khô ráo. – Sản phẩm bị ướt có thể phơi đến khi sản phẩm khô hoàn toàn. – Vệ sinh dùng khăn ẩm hoặc bàn chải sợi đánh qua cho bong đi những chỗ mốc, bụi, bẩn… – Tuổi thọ sản phẩm phụ thuộc vào cách sử dụng & bảo quản của mỗi người. – Chất liệu tự nhiên dù đã qua xử lý cũng sẽ dễ bị ẩm mốc khi điều kiện bảo quản không tốt. #khaymay #khaymaykhamtrai #khaymaytrangtri #khaymaycaocap #khaymaydep #domaytredan #domaytre #khaymaytredan #khaymayguot #khaymaynhabep #dungcunhabep #dunghoaqua #dungdoan #khaymaydunghoaqua", Category = "Gia dụng" },
    new { Name = "Giỏ mây hình chữ nhật đáy vát đựng hoa quả, bánh kẹo", Price = 135000, Description = "Hộp Mây Chữ Nhật Sứ Có Quai Đa Năng Đựng Đồ, Decor phong cách Vintage ✔ Giỏ đựng đồ tiện dụng bằng mây đan được làm từ làng nghề thủ công ✔ Nguyên liệu được làm hoàn toàn từ thiên nhiên đã qua xử lý chống mối mọt, hoàn toàn thân thiện với môi trường ✔ Sản phẩm tiện lợi nhiều chức năng, dùng để đựng rau, củ, quả, thực phẩm khô hoặc các loại văn phòng phẩm, mỹ phẩm… ✔ Nên sử dụng thước kẻ để đo, đừng tưởng tượng kích thước sản phẩm. THÔNG TIN SẢN PHẨM: – Sản phẩm được làm thủ công từ những bàn tay của các nghệ nhân lành nghề của làng nghề thủ công Việt Nam – Chất liệu mây guột đan tự nhiên Kích thước hình hộp chữ nhật : Dài: 30cm x Rộng: 20cm x Cao: 16cm (chưa tính quai) – Sản phẩm thân thiện với môi trường, an toàn cho người sử dụng. * Thích hợp cho: – Dùng làm quà tặng: Cho công ty, gia đình, Cá nhân, nhà hàng, khách sạn – Đựng bánh, kẹo, mứt, trà, trái cây – Trang trí, Decor, sáng tạo cho nhiều mục đích khác nhau – Chụp hình sản phẩm, trang trí phong cách mộc mạc * CÁCH BẢO QUẢN SẢN PHẨM : – Giữ sản phẩm nơi thoáng mát, khô ráo. – Sản phẩm bị ướt có thể phơi đến khi sản phẩm khô hoàn toàn. – Vệ sinh dùng khăn ẩm hoặc bàn chải sợi đánh qua cho bong đi những chỗ mốc, bụi, bẩn… – Tuổi thọ sản phẩm phụ thuộc vào cách sử dụng & bảo quản của mỗi người. – Chất liệu tự nhiên dù đã qua xử lý cũng sẽ dễ bị ẩm mốc khi điều kiện bảo quản không tốt. #khaymay #khaymaykhamtrai #khaymaytrangtri #khaymaycaocap #khaymaydep #domaytredan #domaytre #khaymaytredan #khaymayguot #khaymaynhabep #dungcunhabep #dunghoaqua #dungdoan #khaymaydunghoaqua", Category = "Gia dụng" },
    new { Name = "Khay mây lượn sóng đựng hoa quả, bánh kẹo, đồ khô phòng khách, nhà bếp", Price = 190000, Description = "Khay mây lượn sóng đựng hoa quả, bánh kẹo, đồ khô phòng khách, nhà bếp ✔ Bộ khay đựng đồ tiện dụng bằng mây đan được làm từ làng nghề thủ công ✔ Nguyên liệu được làm hoàn toàn từ thiên nhiên đã qua xử lý chống mối mọt, hoàn toàn thân thiện với môi trường ✔ Sản phẩm tiện lợi nhiều chức năng, dùng để đựng rau, củ, quả, thực phẩm khô hoặc các loại văn phòng phẩm, mỹ phẩm… ✔ Nên sử dụng thước kẻ để đo, đừng tưởng tượng kích thước sản phẩm. THÔNG TIN SẢN PHẨM: – Sản phẩm được làm thủ công từ những bàn tay của các nghệ nhân lành nghề của làng nghề thủ công Việt Nam – Chất liệu mây tre đan tự nhiên – Sản phẩm thân thiện với môi trường, an toàn cho người sử dụng. * Thích hợp cho: – Dùng làm quà tặng: Cho công ty, gia đình, Cá nhân, nhà hàng, khách sạn – Đựng bánh, kẹo, mứt, trà, trái cây – Trang trí, Decor, sáng tạo cho nhiều mục đích khác nhau – Chụp hình sản phẩm, trang trí phong cách mộc mạc * CÁCH BẢO QUẢN SẢN PHẨM : – Giữ sản phẩm nơi thoáng mát, khô ráo. – Sản phẩm bị ướt có thể phơi đến khi sản phẩm khô hoàn toàn. – Vệ sinh dùng khăn ẩm hoặc bàn chải sợi đánh qua cho bong đi những chỗ mốc, bụi, bẩn… – Tuổi thọ sản phẩm phụ thuộc vào cách sử dụng & bảo quản của mỗi người. – Chất liệu tự nhiên dù đã qua xử lý cũng sẽ dễ bị ẩm mốc khi điều kiện bảo quản không tốt. #khaymay #khaymaykhamtrai #khaymaytrangtri #khaymaycaocap #khaymaydep #domaytredan #domaytre #khaymaytredan #khaymayguot #khaymaynhabep #dungcunhabep #dunghoaqua #dungdoan #khaymaydunghoaqua", Category = "Gia dụng" },

    // Trang trí
    new { Name = "Xích đu mây decor sân vườn, ban công, phòng khách cao cấp", Price = 3500000, Description = "THÔNG TIN SẢN PHẨM: Chất liệu: 100% mây tự nhiên Kích thước: Cao 100cm, rộng 75cm Màu sắc: Nâu/ Vàng/ Mây tự nhiên Tải trọng: 80-100kg Tuổi thọ: 5-7 năm Bào hành: 12 tháng 1. MANG LẠI NÉT ĐỘC ĐÁO, THU HÚT CHO KHÔNG GIAN NGÔI NHÀ BẠN Giữa rất nhiều các ngôi nhà mà người thân, bạn bè của bạn đã từng ghé qua thì ngôi nhà có xích đu sẽ làm cho họ ấn tượng nhất, và càng thích thú hơn khi đó là một chiếc xích đu với kiểu dáng độc đáo, được làm tỉ mỉ từ chất liệu mây cao cấp, nó làm nổi bật lên nét sang trọng và khác biệt của không gian sống của bạn mà không phải ngôi nhà nào cũng có được. Bạn có thể kết hợp cùng với đèn mây hay tủ mây để làm không gian thêm lãng mạn và vintage, chuẩn gu cho những cô nàng bánh bèo hay những người yêu thích phong cách sống tối giản. 2. MANG LẠI CẢM GIÁC THƯ GIÃN TUYỆT VỜI CHO CẢ GIA ĐÌNH Hãy tưởng tượng sau một ngày làm việc mệt mỏi được, thả mình trên chiếc xích đu mây đu đưa nhẹ nhàng nhâm nhi ly cafe hay nghe một bản nhạc yêu thích. Mọi muộn phiền, mệt nhọc giường như tan biến hết, bỏ lại sau lưng muôn nẻo bộn bề. Xích đu còn là nơi cho bé yêu được thỏa sức vui đùa, nô nghịch và có một ký ức tuổi thơ đầy ý nghĩa. 3. LẮP ĐẶT TIỆN LỢI VÀ DỄ SỬ DỤNG Xích đu mây tại Papasan Việt Nam còn có thêm chân sắt đi kèm (được khuyên dùng). Chính vì vậy bạn có thể dễ dàng thay đổi vị trí Lắp trực tiếp lên trần hoặc thanh xà của ngôi nhà bạn (cần lắp móc treo) Để tạo nên một mẫu xích đu mây hoàn hảo, người nghệ nhân làng nghề đã phải làm miệt mài trong hơn 50 tiếng. Từ bước chọn lọc kỹ càng những cây mây có tuổi đời 12-14 năm, đến công đoạn sấy mây đúng tiêu chuẩn, tạo khung cho đến bước sơn phủ. Tất cả phải cần được tuân thủ chặt chẽ theo thời gian nhất định. Để cho ra chất lượng sản phẩm bóng mịn, màu tự nhiên, tuổi thọ từ 5-7 năm sử dụng", Category = "Trang trí" },
    new { Name = "Đèn lồng tre đan thủ công dùng trang trí nhà hàng, quán cafe, decor nhà cửa", Price = 680000, Description = "Đèn lồng tre đan thủ công ECOLOOM dùng để trang trí nhà hàng, quán cafe, decor nhà cửa – Màu sắc: Vàng tự nhiên – Chất liệu: Tre Kích thước: + 38x25cm + 37x30cm + 68x36cm – Sản phẩm không bao gồm bóng đèn và dây điện – Đèn dùng để trang trí, decor nhà hàng, quán cafe, resort,… mang lại 1 không gian sang trọng, ấm cúng mang đậm vẻ đẹp của tự nhiên – Đèn có thể sử dụng đui đèn bằng điện hoặc có thể sử dụng nến tuỳ vào khung cảnh sử dụng của mỗi người để làm nổi bật lên vẻ đẹp của chúng – Sản phẩm có độ bền cao ❤️ HƯỚNG DẪN SỬ DỤNG VÀ BẢO QUẢN SẢN PHẨM ❤️ – Để nơi khô ráo, tránh các chất tẩy rửa – Sử dụng khăn ẩm để lau các vết dơ trong quá trình sử dụng, không ngâm sản phẩm vào nước – Phơi nắng sau khi lau bằng khăn ẩm –––––––––––––––– ❁ BẢO HÀNH & DỊCH VỤ – Bảo hành 1 đổi 1 miễn phí trong vòng7 ngày nếu sản phẩm có lỗi kĩ thuật. – Bảo hành chất lượng sản phẩm 12 tháng. #dentre #denlongtre #dentrecoi #dencoi #dendecor #dentrangtri #denvintage #denongtre", Category = "Trang trí" },
    new { Name = "Chụp đèn trang trí hình tổ chim", Price = 540000, Description = "Chao đèn mây tre đan hình tròn. Chụp đèn tròn trang trí decor phòng ngủ phòng ăn phòng khách, quán ăn nhà hàng khách sạn. Thân thiện môi trường / Bán lẻ bán buôn ” Chụp đèn / Chao đèn này được những người thợ đan tỉ mỉ từ những sợi mây hoặc tre chất lượng cao. Mỗi sản phẩm được tạo bởi nhiều dáng hình phù hợp với nhiều không gian như: nhà ở, nhà hàng, khách sạn, resort, homestay, quán cafe…” –––––––––––––––– THÔNG TIN CHUNG : – Chất lượng cao, thời gian sử dụng lâu dài. – Thân thiện với môi trường. – Mang phong cách tối giản, truyền thống. –––––––––––––––– THÔNG SỐ KỸ THUẬT : – Kiểu dáng : Hình tổ chim – Màu sắc : Màu tự nhiên – Chất liệu : Mây – Phụ kiện : không bao gồm bóng đèn – Quy trình sản xuất: Thủ công – Xuất xứ: Hà Nội, Việt Nam –––––––––––––––– BẢO QUẢN : – Tránh ánh nắng trực tiếp, nhiệt độ cao. – Lau chùi bằng khăn mềm. – Để nơi khô ráo, thoáng mát, tránh ngâm trong nước hoặc để ở khu vực ẩm thấp. –––––––––––––––– LƯU Ý : – Vì đây là sản phẩm handmade, làm bằng tay, thủ công; do vậy đôi khi sẽ có một chút chênh lệch so với ảnh. – Xưởng nhận sản xuất theo kích thước và mẫu mã yêu cầu. – Khách hàng có nhu cầu làm CTV, đại lý hoặc mua sỉ, hãy liên hệ với shop nhé. – Mọi thông tin chi tiết và cần tư vấn, quý khách hãy vào mục CHAT trên Shopee. –––––––––––––––– #chupden #chaoden #chupdentrangtri #denchupquanan #chupdenbangmaytre #chupdenbangmay #chupdenbangtre #longchupden #longden #chaodenmaytre #chaodenngoaitroi #chaodenngu #chupdenmay #chupdenmaytre #chupdenngu", Category = "Trang trí" },
    new { Name = "Đèn tre lồng tròn", Price = 540000, Description = "THÔNG TIN CHUNG : – Chất lượng cao, thời gian sử dụng lâu dài. – Thân thiện với môi trường. – Mang phong cách tối giản, truyền thống. – Loại: thả trần. – Vật Liệu: tre – Kích thước: 30cm, 40cm – Xuất xứ: Việt Nam Lưu ý: sản phẩm trên không bao gồm bóng điện, dây điện và đui đèn LƯU Ý : – Vì đây là sản phẩm handmade, làm bằng tay, thủ công; do vậy đôi khi sẽ có một chút chênh lệch so với ảnh. – Xưởng nhận sản xuất theo kích thước và mẫu mã yêu cầu. – Sản phẩm trên không bao gồm bóng điện, dây điện và đui đèn", Category = "Trang trí" },
    new { Name = "Đèn Trụ Dài Làm Từ Khung Tre Có Lớp Vải Thả Trang Trí", Price = 540000, Description = "Đèn Trụ Dài Làm Từ Khung Tre Có Lớp Vải Thả Trang Trí với chất liệu chính là từ mây tre tự nhiên trải qua nhiều công đoạn xử lí cộng với kiểu dáng thiết kế độc đáo, mang đậm phong cách giản dị nhưng không kém phần sang trọng hiện đại sẽ là một điểm nhấn nổi bật trong ngôi nhà của bạn. 1. Thông tin sản phẩm đèn mây tre – Kích thước: tùy theo yêu cầu khách hàng – Chất liệu: 100% bằng mây tự nhiên – Có 1 lớp vải tán sáng màu trắng phía trong – Màu sắc: tự nhiên, – Đặc biệt sản phẩm còn được tiến hành xử lý chống mối mọt. 2. Đặc điểm nổi bật của đèn mây tre – Là một loại dùng để thả trần, nó được kết hợp từ nhiều nan mây tre đan xen với nhau rất tỉ mĩ – Có thể được bắt gặp rất nhiều với các mẫu mã thiết kế khác nhau. Một trong những công dụng của nó là làm chan hòa ánh sáng cho không gian trong gia đình. – Dùng để tạo không gian riêng tư; tạo cảm giác ấm áp, là một vật dụng để trang trí thêm cho căn phòng một nét riêng biệt và một không gian trữ tình hay lãng mạn. – Được gia công tỉ mỉ từ các nghệ lành nghề cho ra mẫu đèn chất lượng và mang giá trị thẫm mỹ cao trước khi đến tay người dùng. – Những chiếc đèn mây tre được sử dụng rộng rãi trong các quán ăn, nhà hàng, khu resort, tiệm spa, khu du lịch, homestay, farmstay…vì tính thẩm mỹ, dân giã và đậm chất nghệ thuật trang trí lôi cuốn", Category = "Trang trí" },

    // Thời trang
    new { Name = "Túi xách bện mây cho nữ", Price = 439000, Description = "28 x 21 x 9 cm Mây màu mật ong đậm Chi tiết hình vỏ sò Xin lưu ý, do giỏ của chúng tôi được làm thủ công nên số đo chúng tôi cung cấp chỉ mang tính chất tham khảo, vì vậy vui lòng cho phép chênh lệch +/- 5% Bảo dưỡng & Vệ sinh Mây rất bền và có thể chịu được sự thay đổi nhiệt độ. Để vệ sinh hàng ngày, hãy lau bằng vải mềm. Để vệ sinh kỹ hơn, hãy sử dụng vải ẩm có chất tẩy rửa nhẹ, lau bề mặt nhưng không làm ướt mây. Khi mây rất ẩm, tránh uốn cong mây. Thay vào đó, hãy đặt đồ nội thất mây ngoài trời để khô thoáng. Tìm hiểu thêm tại đây.. Điểm đến giao hàng Chúng tôi cung cấp dịch vụ giao hàng trên toàn thế giới. Thời gian xử lý đơn hàng Chúng tôi thường mất 1-3 ngày để xử lý đơn hàng.", Category = "Thời trang" },
    new { Name = "Túi mây tre đan tròn sáng quai da", Price = 160000, Description = "Túi mây thủ công hình trống tròn đan họa tiết hoa văn phong cách Vintage Xuất xứ : Việt Nam Sản phẩm được làm bằng thủ công từ các làng nghề mây tre đan Việt Nam, xuất khẩu và rất được ưa chuộng tại các nước Châu Âu, Đông Nam Á,Trung quốc, Hàn quốc, Nhật Bản... Chất liệu : Mây, ruột mây tự nhiên đã xử lý mối mọt và phun bóng Dây đeo : làm bằng da Pu, dây đeo may dính liền túi, chiều dài dây cố định Lớp lót : Vải canvas, màu nâu (kaki), màu xanh (blue) Nắp khóa : da Pu, kèm đinh tán Kích thước 18cm Nếu quý khách có thắc mắc vui lòng inbox - Mây Mây hân hạnh phục vụ #túi mây#túi đi biển #túi du lịch", Category = "Thời trang" },
    new { Name = "Túi mây tre đan tròn họa tiết viền hoa", Price = 510000, Description = "Túi được đan bằng sợi mây tự nhiên. Form túi cứng. Kích thước: Đường kính 18cm Túi cói lót vải và dây đeo da dài.", Category = "Thời trang" },
    new { Name = "Túi xách mây tre đan, hình chữ nhật, có quai sách", Price = 510000, Description = "- Túi Mây Tre Đan được thiết kế tinh xảo, thanh lịch có thể cho thấy khí chất vẻ đẹp của bạn. Nó rất phù hợp cho đám cưới, tiệc tùng, mặc thường ngày, đi du lịch, dịp công sở. Món quà tuyệt vời cho bạn bè, gia đình hoặc chính bạn. - Kích thước gần đúng: 19x13h7. Size Kích thước có thể hơi khác nhau do đo thủ công. - Chúng tôi có thể đảm bảo màu sắc của những chiếc túi hoàn toàn giống hệt như được làm bằng mây. - Sức chứa: Có thể đựng điện thoại di động, ví, son môi và các vật dụng hằng ngày... Nếu bạn là một tín đồ thời trang, đây là sự lựa chọn tốt nhất cho bạn. Chiếc túi này mang đến sự tự nhiên, độc đáo và sang trọng. - Chất liệu : Làm từ sợi mây 100% tự nhiên với đan thủ công tinh xảo. - Chức năng: Đeo vai và đeo chéo - Ưu điểm: Vẻ đẹp mộc mạc, tự nhiên, thân thiện với môi trường. Đường đan tỉ mỉ, mang đến độ bền đẹp theo thời gian. Dễ dàng vệ sinh, tiết kiệm thời gian. - Bảo quản - Bảo quản nơi thoáng mát, khô ráo sau khi sử dụng. Nếu bị ướt có thể phơi nắng hoặc sấy khô. Đia chỉ : . Phú Nghĩa _ Chương Mỹ _ Hà Nội #maytredanhandmade #rattan #diy #handmade #bag #tuixach #khuyentai #lotly #coasters #decor #khaymay #tuimay #maytredan", Category = "Thời trang" },
    new { Name = "Túi xách mây tre hình tròn", Price = 900000, Description = "Nâng tầm phong cách mùa hè của bạn với Túi rơm Mini Rattan, được làm thủ công từ rơm mây tự nhiên. Kích thước nhỏ gọn của nó lý tưởng để đựng những vật dụng thiết yếu như điện thoại, chìa khóa, ví và kem chống nắng, đồng thời thêm nét quyến rũ theo phong cách bohemian cho bất kỳ trang phục nào. Tính năng & Kích thước Kích thước: 20 x 20 x 8 cm 7,9″ ✕ 7,9″ ✕ 3,1″ Mây màu mật ong đậm Chi tiết hình vỏ sò Xin lưu ý, do giỏ của chúng tôi được làm thủ công, nên số đo chúng tôi cung cấp chỉ mang tính chất tham khảo, vì vậy vui lòng cho phép chênh lệch +/- 5%. Màu sắc cũng có thể khác so với hình ảnh hiển thị vì sản phẩm của chúng tôi được làm từ vật liệu tự nhiên.", Category = "Thời trang" }
};
            return JsonConvert.SerializeObject(products);
        }
    }
}