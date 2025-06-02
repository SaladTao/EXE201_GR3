 
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;

namespace exe201.Pages.Home
{
    public class StoryModel : PageModel
    {
        public List<Story> Stories { get; set; }

        public void OnGet()
        {
            Stories = new List<Story>
            {
                new Story
                {
                    Id = 1,
                    Title = "Câu truyện về đèn lồng mây tre đan",
                    ImageUrl = "https://maytretrungphuong.com/wp-content/uploads/2021/09/Den-hat-cuom-ruc-ro-dep-mat.png",
                    Description = "Đèn lồng mây tre đan không chỉ là một sản phẩm thủ công mỹ nghệ, mà còn là biểu tượng của sự sáng tạo và khéo léo. Những chiếc đèn lồng được làm từ tre và mây tự nhiên, mang lại ánh sáng ấm áp và vẻ đẹp mộc mạc cho không gian sống."
                },
                new Story
                {
                    Id = 2,
                    Title = "Câu truyện về người dân làng nghề mây tre đan",
                    ImageUrl = "https://file3.qdnd.vn/data/images/0/2022/10/27/vuhuyen/may-tre.jpg?dpi=150&quality=100&w=870",
                    Description = "Người dân làng nghề mây tre đan đã gìn giữ nghề truyền thống qua nhiều thế hệ. Họ làm việc với niềm đam mê, biến những sợi mây tre thô sơ thành các sản phẩm tinh xảo, mang đậm dấu ấn văn hóa Việt Nam."
                },
                new Story
                {
                    Id = 3,
                    Title = "Nghệ thuật mây tre đan qua từng thế hệ",
                    ImageUrl = "https://nguoinamdinh.net/wp-content/uploads/2016/06/1-128.jpg",
                    Description = "Nghệ thuật mây tre đan không chỉ là một nghề, mà còn là một di sản văn hóa được truyền từ thế hệ này sang thế hệ khác. Mỗi sản phẩm đều chứa đựng câu chuyện về sự kiên nhẫn và tình yêu với nghề của các nghệ nhân."
                },
                new Story
                {
                    Id = 4,
                    Title = "Hành trình từ tre làng đến sản phẩm tinh xảo",
                    ImageUrl = "https://dntt.mediacdn.vn/197608888129458176/2023/12/16/lang-may-tre-dan-phu-vinh-11675520593-1702739108477352274759.jpeg",
                    Description = "Từ những cây tre mọc bên bờ sông, qua bàn tay khéo léo của nghệ nhân, từng sợi tre được xử lý tỉ mỉ để tạo ra các sản phẩm tinh xảo như giỏ, bàn ghế, và đồ trang trí, mang lại giá trị cả về thẩm mỹ và thực tiễn."
                },
                new Story
                {
                    Id = 5,
                    Title = "Mây tre đan trong đời sống hàng ngày",
                    ImageUrl = "https://dentrangtrimaianh.com/wp-content/uploads/2023/11/Diem-danh-15-lang-nghe-may-tre-dan-truyen-thong-noi-tieng-khap-ca-nuoc-Noi-tao-nen-nhung-mon-do-nghe-thuat-va-mang-lai-nguon-thu-nhap-dang-ke-cho-nguoi-dan-dia-phuong.jpg",
                    Description = "Các sản phẩm mây tre đan đã trở thành một phần không thể thiếu trong đời sống hàng ngày của người Việt, từ những chiếc giỏ đựng đồ, bàn ghế mây, đến các vật dụng trang trí, tất cả đều mang vẻ đẹp mộc mạc và gần gũi."
                },
                new Story
                {
                    Id = 6,
                    Title = "Sự đổi mới trong nghề mây tre đan",
                    ImageUrl = "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcR73am97KcUjDZpOM09W1T0MBlPMauz9WscaA&s",
                    Description = "Ngày nay, nghề mây tre đan không chỉ giữ gìn truyền thống mà còn đổi mới với các thiết kế hiện đại, kết hợp với các vật liệu khác để tạo ra các sản phẩm phù hợp với thị hiếu và nhu cầu của thời đại mới."
                },
                new Story
                {
                    Id = 7,
                    Title = "Mây tre đan và môi trường bền vững",
                    ImageUrl = "https://vungdecor.com/wp-content/uploads/2024/12/do-may-tre-dan-hcm2.png",
                    Description = "Mây tre đan là một giải pháp thân thiện với môi trường, sử dụng nguyên liệu tự nhiên và tái tạo. Các sản phẩm mây tre không chỉ bền đẹp mà còn góp phần giảm thiểu tác động đến môi trường, hướng tới một lối sống bền vững."
                }
            };
        }
    }

    public class Story
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string ImageUrl { get; set; }
        public string Description { get; set; }
    }
}
 