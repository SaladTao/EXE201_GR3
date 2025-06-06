using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using System.Linq;

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
            var allStories = GetAllStories();

            Categories = allStories.Select(s => s.Category).Distinct().OrderBy(c => c).ToList();

            var filteredStories = string.IsNullOrEmpty(SelectedCategory)
                ? allStories.OrderBy(s => s.Title).ToList()
                : allStories.Where(s => s.Category == SelectedCategory).OrderBy(s => s.Title).ToList();

            TotalPages = (int)Math.Ceiling(filteredStories.Count / (double)PageSize);

            var pagedStories = filteredStories
                .OrderByDescending(s => s.PostedDate)
                .Skip((PageNumber - 1) * PageSize)
                .Take(PageSize)
                .ToList();

            // Gán lại Id theo vị trí thực tế trên toàn bộ danh sách
            for (int i = 0; i < pagedStories.Count; i++)
            {
                pagedStories[i].Id = (PageNumber - 1) * PageSize + i + 1;
            }

            Stories = pagedStories;
        }


        private List<Story> GetAllStories()
        {
            return new List<Story>
            {
                new Story
                {
                    Title = "Câu truyện về đèn lồng mây tre đan",
                    ImageUrl = "https://maytretrungphuong.com/wp-content/uploads/2021/09/Den-hat-cuom-ruc-ro-dep-mat.png",
                    Description = "Đèn lồng mây tre đan không chỉ là một sản phẩm thủ công mỹ nghệ, mà còn là biểu tượng của sự sáng tạo và khéo léo. Những chiếc đèn lồng được làm từ tre và mây tự nhiên, mang lại ánh sáng ấm áp và vẻ đẹp mộc mạc cho không gian sống.",
                    PostedDate = new DateTime(2025, 1, 15),
                    Category = "Sản phẩm thủ công"
                },
                new Story
                {
                    Title = "Câu truyện về người dân làng nghề mây tre đan",
                    ImageUrl = "https://file3.qdnd.vn/data/images/0/2022/10/27/vuhuyen/may-tre.jpg?dpi=150&quality=100&w=870",
                    Description = "Người dân làng nghề mây tre đan đã gìn giữ nghề truyền thống qua nhiều thế hệ. Họ làm việc với niềm đam mê, biến những sợi mây tre thô sơ thành các sản phẩm tinh xảo, mang đậm dấu ấn văn hóa Việt Nam.",
                    PostedDate = new DateTime(2025, 2, 10),
                    Category = "Làng nghề"
                },
                new Story
                {
                    Title = "Nghệ thuật mây tre đan qua từng thế hệ",
                    ImageUrl = "https://nguoinamdinh.net/wp-content/uploads/2016/06/1-128.jpg",
                    Description = "Nghệ thuật mây tre đan không chỉ là một nghề, mà còn là một di sản văn hóa được truyền từ thế hệ này sang thế hệ khác. Mỗi sản phẩm đều chứa đựng câu chuyện về sự kiên nhẫn và tình yêu với nghề của các nghệ nhân.",
                    PostedDate = new DateTime(2025, 3, 5),
                    Category = "Di sản văn hóa"
                },
                new Story
                {
                    Title = "Hành trình về sản phẩm tinh xảo",
                    ImageUrl = "https://dntt.mediacdn.vn/197608888129458176/2023/12/16/lang-may-tre-dan-phu-vinh-11675520593-1702739108477352274759.jpeg",
                    Description = "Từ những cây tre mọc bên bờ sông, qua bàn tay khéo léo của nghệ nhân, từng sợi tre được xử lý tỉ mỉ để tạo ra các sản phẩm tinh xảo như giỏ, bàn ghế, và đồ trang trí, mang lại giá trị cả về thẩm mỹ và thực tiễn.",
                    PostedDate = new DateTime(2025, 4, 20),
                    Category = "Quy trình sản xuất"
                },
                new Story
                {
                    Title = "Mây tre đan trong đời sống hàng ngày",
                    ImageUrl = "https://dentrangtrimaianh.com/wp-content/uploads/2023/11/Diem-danh-15-lang-nghe-may-tre-dan-truyen-thong-noi-tieng-khap-ca-nuoc-Noi-tao-nen-nhung-mon-do-nghe-thuat-va-mang-lai-nguon-thu-nhap-dang-ke-cho-nguoi-dan-dia-phuong.jpg",
                    Description = "Các sản phẩm mây tre đan đã trở thành một phần không thể thiếu trong đời sống hàng ngày của người Việt, từ những chiếc giỏ đựng đồ, bàn ghế mây, đến các vật dụng trang trí, tất cả đều mang vẻ đẹp mộc mạc và gần gũi.",
                    PostedDate = new DateTime(2025, 5, 12),
                    Category = "Ứng dụng thực tiễn"
                },
                new Story
                {
                    Title = "Sự đổi mới trong nghề mây tre đan",
                    ImageUrl = "https://encrypted-tbn0.gstatic.com/images?q=tb&n:ANd9GcR73am97KcUjDZpOM09W1T0MBlPMauz9WscaA&s",
                    Description = "Ngày nay, nghề mây tre đan không chỉ giữ gìn truyền thống mà còn đổi mới với các thiết kế hiện đại, kết hợp với các vật liệu khác để tạo ra các sản phẩm phù hợp với thị hiếu và nhu cầu của thời đại mới.",
                    PostedDate = new DateTime(2025, 6, 1),
                    Category = "Đổi mới sáng tạo"
                },
                new Story
                {
                    Title = "Mây tre đan và môi trường bền vững",
                    ImageUrl = "https://vungdecor.com/wp-content/uploads/2024/12/do-may-tre-dan-hcm2.png",
                    Description = "Mây tre đan là một giải pháp thân thiện với môi trường, sử dụng nguyên liệu tự nhiên và tái tạo. Các sản phẩm mây tre không chỉ bền đẹp mà còn góp phần giảm thiểu tác động đến môi trường, hướng tới một lối sống bền vững.",
                    PostedDate = new DateTime(2025, 6, 5),
                    Category = "Bảo vệ môi trường"
                },
                new Story
                {
                    Title = "Phú Vinh – Nơi mây tre kể chuyện nghìn năm 🌾",
                    ImageUrl = "",
                    Description = "Về một làng nghề mang hồn Việt…\r\nCách trung tâm Hà Nội chưa đầy 30km, làng Phú Vinh (xã Phú Nghĩa, huyện Chương Mỹ) từ lâu đã được mệnh danh là “cái nôi của nghệ thuật mây tre đan Việt Nam”. Nơi đây không chỉ đơn thuần là một làng nghề, mà là cả một kho tàng sống động về kỹ thuật, văn hóa và tinh thần thủ công truyền thống.\r\nTừ đôi tay khéo léo của những người nghệ nhân, những sợi mây, sợi tre tưởng chừng đơn giản đã hóa thành giỏ, khay, đèn, tranh, vật dụng decor… không chỉ phục vụ đời sống hàng ngày mà còn có mặt tại nhiều thị trường quốc tế.\r\n\U0001f9fa Điều làm nên đặc biệt của mây tre Phú Vinh chính là:\r\n  ✓ Hoa văn tinh xảo, kết cấu chắc chắn, mang tính nghệ thuật cao\r\n  ✓ Sự cải tiến trong thiết kế, kết hợp truyền thống với xu hướng hiện đại\r\n  ✓ Và hơn hết, là tâm huyết truyền nghề từ đời này sang đời khác\r\n🌿 Ecoloom tự hào khi được kết nối và hợp tác với các nghệ nhân làng Phú Vinh – những người vẫn ngày đêm gìn giữ “chất quê” qua từng sợi mây đan. Mỗi sản phẩm đến từ Ecoloom là một phần của Phú Vinh – một phần của văn hóa Việt.\r\n📸 Bạn đã từng ghé thăm Phú Vinh chưa? Nếu chưa, hãy cùng chúng mình đi qua những câu chuyện làng nghề trong các bài viết sắp tới nhé!\r\n#Ecoloom #PhuVinh #LangNgheTruyenThong #MayTreDan #NgheNhanViet #VanHoaViet #SongXanh\r\n",
                    PostedDate = new DateTime(2025, 6, 5),
                    Category = "Sự tích"
                },
                new Story
                {
                    Title = "Tại sao nên chọn sản phẩm thân thiện môi trường?",
                    ImageUrl = "",
                    Description = "",
                    PostedDate = new DateTime(2025, 6, 5),
                    Category = "Bảo vệ môi trường"
                },
                new Story
                {
                    Title = "Cùng ecoloom decor cho không gian xanh",
                    ImageUrl = "",
                    Description = "",
                    PostedDate = new DateTime(2025, 6, 5),
                    Category = "Ecoloom"
                },
                new Story
                {
                    Title = "Top 5 vật dụng tre giúp decor nhà xinh",
                    ImageUrl = "",
                    Description = "",
                    PostedDate = new DateTime(2025, 6, 5),
                    Category = "Trang trí"
                }
                ,new Story
                {
                    Title = "1 ngày của nghệ nhân đan mây",
                    ImageUrl = "",
                    Description = "",
                    PostedDate = new DateTime(2025, 6, 5),
                    Category = "Câu chuyện"
                }
                ,new Story
                {
                    Title = "Câu chuyện khách hàng – Sống xanh bắt đầu từ điều nhỏ nhấ",
                    ImageUrl = "",
                    Description = "",
                    PostedDate = new DateTime(2025, 6, 5),
                    Category = "Câu chuyện"
                }
                ,new Story
                {
                    Title = "Mẹo bảo quản sản phẩm tre",
                    ImageUrl = "",
                    Description = "",
                    PostedDate = new DateTime(2025, 6, 5),
                    Category = "Mẹo"
                }
                ,new Story
                {
                    Title = "Quote hay về sống xanh, tối giản",
                    ImageUrl = "",
                    Description = "",
                    PostedDate = new DateTime(2025, 6, 5),
                    Category = "Quote"
                }
                ,new Story
                {
                    Title = "Mỗi căn nhà – Một bản sắc sống xanh riêng biệt",
                    ImageUrl = "",
                    Description = "",
                    PostedDate = new DateTime(2025, 6, 5),
                    Category = "Trang trí"
                }
                ,new Story
                {
                    Title = "Bạn không cần phải cầu kỳ để nổi bật – chỉ cần một chiếc túi mây xinh thủ công là đủ.",
                    ImageUrl = "",
                    Description = "",
                    PostedDate = new DateTime(2025, 6, 5),
                    Category = "Câu chuyện"
                }
                ,new Story
                {
                    Title = "Đồ gia dụng tre – sống xanh từ bếp",
                    ImageUrl = "",
                    Description = "",
                    PostedDate = new DateTime(2025, 6, 5),
                    Category = "Câu chuyện"
                }
                ,new Story
                {
                    Title = "Túi mây thủ công – mỗi chiếc là một tác phẩm",
                    ImageUrl = "",
                    Description = "",
                    PostedDate = new DateTime(2025, 6, 5),
                    Category = "Câu chuyện"
                }
                ,new Story
                {
                    Title = "Trang sức mây – Nét chấm phá cho nàng nhẹ nhàng",
                    ImageUrl = "",
                    Description = "",
                    PostedDate = new DateTime(2025, 6, 5),
                    Category = "Trang sức"
                }
                ,new Story
                {
                    Title = "Tủ đồ “Xanh” cho cô nàng yêu thiên nhiên",
                    ImageUrl = "",
                    Description = "",
                    PostedDate = new DateTime(2025, 6, 5),
                    Category = "Bảo vệ môi trường"
                }
                ,new Story
                {
                    Title = "Bộ đôi hoàn hảo: Túi xách + phụ kiện mây",
                    ImageUrl = "",
                    Description = "",
                    PostedDate = new DateTime(2025, 6, 5),
                    Category = "Trang trí"
                }
                ,new Story
                {
                    Title = "Review từ khách hàng thật",
                    ImageUrl = "",
                    Description = "",
                    PostedDate = new DateTime(2025, 6, 5),
                    Category = "Review"
                }
            };
        }
    }

    public class Story
    {
        public int Id { get; set; }
        public int DisplayId { get; set; }
        public string Title { get; set; }
        public string ImageUrl { get; set; }
        public string Description { get; set; }
        public DateTime PostedDate { get; set; }
        public string Category { get; set; }
    }
}