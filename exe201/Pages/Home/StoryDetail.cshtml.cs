using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Linq;

namespace exe201.Pages.Home
{
    public class StoryDetailModel : PageModel
    {
        public Story Storys { get; set; }
        [BindProperty(SupportsGet = true)]
        public int PageNumber { get; set; }

        [BindProperty(SupportsGet = true, Name = "selectedCategory")]
        public string SelectedCategory { get; set; }

        private static readonly List<Story> Stories = new List<Story>
        {
            new Story
            {
                Id = 1,
                Title = "Câu truyện về đèn lồng mây tre đan",
                ImageUrl = "https://maytretrungphuong.com/wp-content/uploads/2021/09/Den-hat-cuom-ruc-ro-dep-mat.png",
                Description = "Đèn lồng mây tre đan không chỉ là một sản phẩm thủ công mỹ nghệ, mà còn là biểu tượng của sự sáng tạo và khéo léo của người Việt Nam qua hàng trăm năm. Xuất hiện từ thời xa xưa, những chiếc đèn lồng được làm từ tre và mây tự nhiên, được đan bằng tay với sự tỉ mỉ và kiên nhẫn của các nghệ nhân. Không chỉ mang lại ánh sáng ấm áp cho các không gian sống như nhà cửa, đình chùa, hay các lễ hội truyền thống, đèn lồng mây tre đan còn thể hiện tinh thần văn hóa dân tộc, đặc biệt trong dịp Tết Trung Thu hay lễ hội đèn lồng ở Hội An. Quá trình làm ra một chiếc đèn lồng không chỉ đòi hỏi kỹ thuật cao mà còn là sự hòa quyện giữa nghệ thuật và thiên nhiên, khi mỗi sợi tre, sợi mây đều được chọn lọc kỹ lưỡng từ những vùng quê giàu tài nguyên. Ngày nay, đèn lồng mây tre đan không chỉ giữ vai trò truyền thống mà còn được cải tiến với các mẫu mã hiện đại, trở thành món quà lưu niệm được yêu thích bởi cả du khách trong và ngoài nước, góp phần quảng bá văn hóa Việt Nam ra toàn cầu."
            },
            new Story
            {
                Id = 2,
                Title = "Câu truyện về người dân làng nghề mây tre đan",
                ImageUrl = "https://file3.qdnd.vn/data/images/0/2022/10/27/vuhuyen/may-tre.jpg?dpi=150&quality=100&w=870",
                Description = "Người dân làng nghề mây tre đan là những người đã gìn giữ và phát huy một trong những di sản văn hóa quý giá của Việt Nam qua nhiều thế hệ. Những ngôi làng như Phú Vinh (Phú Thọ), Vĩnh Nhi (Nghệ An), hay Mỹ Xuyên (Nam Định) từ lâu đã nổi tiếng với nghề đan lát, nơi mà mỗi gia đình đều góp phần vào việc biến những nguyên liệu thô sơ như tre, nứa, và mây thành những sản phẩm tinh xảo. Cuộc sống của họ gắn liền với nhịp điệu của công việc thủ công, từ việc đi lấy nguyên liệu ở rừng, xử lý chúng qua nhiều công đoạn như ngâm, luộc, phơi khô, đến việc đan lát tỉ mỉ dưới ánh đèn dầu hay ánh nắng ban mai. Không chỉ là một nguồn thu nhập, nghề mây tre đan còn là niềm tự hào, là sợi dây kết nối cộng đồng, giúp họ vượt qua khó khăn kinh tế và bảo tồn bản sắc dân tộc. Dù đối mặt với sự cạnh tranh của các sản phẩm công nghiệp, những người nghệ nhân vẫn kiên trì, sáng tạo, và truyền dạy nghề cho thế hệ trẻ, hy vọng giữ gìn truyền thống này mãi mãi."
            },
            new Story
            {
                Id = 3,
                Title = "Nghệ thuật mây tre đan qua từng thế hệ",
                ImageUrl = "https://danviet.mediacdn.vn/296231569849192448/2022/12/23/5-16717652961501377068253.jpg",
                Description = "Nghệ thuật mây tre đan không chỉ là một nghề thủ công mà còn là một di sản văn hóa được truyền từ thế hệ này sang thế hệ khác, phản ánh sự bền bỉ và sáng tạo của người Việt qua hàng trăm năm. Từ những ngày đầu tiên khi tổ tiên ta sử dụng tre, nứa để dựng nhà, làm dụng cụ sinh hoạt, nghệ thuật đan lát đã phát triển thành một ngành nghề chuyên biệt với các sản phẩm đa dạng như rổ, rá, ghế mây, và đồ trang trí. Mỗi thế hệ nghệ nhân đều mang đến những cải tiến nhỏ, từ cách xử lý nguyên liệu để tăng độ bền, đến việc sáng tạo các hoa văn độc đáo, làm nổi bật cá tính của từng vùng miền. Quá trình truyền nghề thường diễn ra trong gia đình, nơi ông bà dạy con cháu từng bước cơ bản, từ cách chọn tre, uốn mây, đến cách đan sao cho chắc chắn và đẹp mắt. Ngày nay, khi công nghệ hiện đại thách thức các nghề truyền thống, nghệ thuật mây tre đan vẫn tồn tại nhờ sự nỗ lực của các nghệ nhân và sự quan tâm của cộng đồng, trở thành một phần không thể thiếu trong bản sắc văn hóa Việt Nam."
            },
            new Story
            {
                Id = 4,
                Title = "Hành trình từ tre làng đến sản phẩm tinh xảo",
                ImageUrl = "https://nongnghiep.vn/images/2023/11/16/20231116161837-1.jpg",
                Description = "Hành trình từ tre làng đến các sản phẩm mây tre đan tinh xảo là một câu chuyện dài về sự kiên trì và sự kết nối giữa con người và thiên nhiên. Mọi thứ bắt đầu từ những cánh đồng tre xanh mướt bên bờ sông, nơi người dân lựa chọn những cây tre già, thẳng để đảm bảo chất lượng. Sau khi được chặt về, tre được ngâm nước, luộc để loại bỏ nhựa, phơi khô dưới ánh nắng tự nhiên trong nhiều ngày để tăng độ bền. Tiếp theo, những sợi tre được tách nhỏ, uốn cong, và đan thành các sản phẩm như giỏ, rổ, hay bàn ghế. Quá trình này đòi hỏi sự khéo léo và kinh nghiệm, khi mỗi đường đan phải đều nhau, mỗi mối nối phải chắc chắn. Từ những nguyên liệu đơn sơ, các nghệ nhân đã tạo ra những sản phẩm không chỉ đẹp mắt mà còn hữu dụng trong đời sống hàng ngày, từ việc đựng nông sản, bảo quản thực phẩm, đến trang trí nội thất. Hành trình này không chỉ là một quy trình sản xuất mà còn là minh chứng cho sự gắn bó giữa con người và thiên nhiên, biến những gì giản dị thành những tác phẩm nghệ thuật."
            },
            new Story
            {
                Id = 5,
                Title = "Mây tre đan trong đời sống hàng ngày",
                ImageUrl = "https://dentrangtrimaianh.com/wp-content/uploads/2023/11/Diem-danh-15-lang-nghe-may-tre-dan-truyen-thong-noi-tieng-khap-ca-nuoc-Noi-tao-nen-nhung-mon-do-nghe-thuat-va-mang-lai-nguon-thu-nhap-dang-ke-cho-nguoi-dan-dia-phuong.jpg",
                Description = "Mây tre đan đã trở thành một phần không thể thiếu trong đời sống hàng ngày của người Việt Nam qua nhiều thế kỷ, mang lại sự tiện ích và vẻ đẹp mộc mạc cho mọi gia đình. Từ những chiếc rổ, rá dùng để đựng gạo, rau củ, đến các loại giỏ đi chợ, giỏ xách, hay bàn ghế mây tre được đặt trong phòng khách, sản phẩm mây tre đan xuất hiện khắp mọi nơi trong nhà cửa và cuộc sống thường nhật. Không chỉ phục vụ nhu cầu thực tế, những sản phẩm này còn mang ý nghĩa văn hóa sâu sắc, thường được sử dụng trong các dịp lễ hội, cưới hỏi, hoặc làm quà tặng. Sự linh hoạt của mây tre đan còn cho phép các nghệ nhân tạo ra các vật dụng trang trí như đèn lồng, bình hoa, hay khung ảnh, biến không gian sống trở nên ấm cúng và gần gũi với thiên nhiên. Với giá thành hợp lý và độ bền cao, mây tre đan không chỉ là lựa chọn phổ biến của người dân nông thôn mà còn được ưa chuộng trong các gia đình thành thị, khẳng định giá trị bền vững của nghề thủ công truyền thống."
            },
            new Story
            {
                Id = 6,
                Title = "Sự đổi mới trong nghề mây tre đan",
                ImageUrl = "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcR73am97KcUjDZpOM09W1T0MBlPMauz9WscaA&s",
                Description = "Sự đổi mới trong nghề mây tre đan là một bước tiến quan trọng giúp nghề truyền thống này thích nghi với thời đại hiện đại và thị trường toàn cầu. Trong những năm gần đây, các nghệ nhân và nhà thiết kế đã bắt đầu kết hợp mây tre đan với các chất liệu mới như gỗ, kim loại, hoặc vải để tạo ra những sản phẩm mang phong cách đương đại, từ ghế mây tối giản đến đồ nội thất cao cấp. Những cải tiến này không chỉ làm tăng tính thẩm mỹ mà còn mở rộng phạm vi ứng dụng, từ đồ dùng gia đình đến trang trí quán cà phê, khách sạn, hay không gian văn phòng. Ngoài ra, các công cụ hiện đại như máy cắt laser cũng được áp dụng để tạo ra các hoa văn phức tạp, giúp tiết kiệm thời gian mà vẫn giữ được chất lượng. Sự đổi mới này không chỉ bảo vệ nghề truyền thống khỏi nguy cơ mai một mà còn tạo ra cơ hội việc làm mới cho thế hệ trẻ, đồng thời đưa sản phẩm mây tre đan Việt Nam vươn xa trên thị trường quốc tế."
            },
            new Story
            {
                Id = 7,
                Title = "Mây tre đan và môi trường bền vững",
                ImageUrl = "https://vungdecor.com/wp-content/uploads/2024/12/do-may-tre-dan-hcm2.png",
                Description = "Mây tre đan không chỉ là một nghề thủ công mà còn là một giải pháp thân thiện với môi trường, góp phần thúc đẩy lối sống bền vững trong bối cảnh biến đổi khí hậu toàn cầu. Tre và mây là những nguyên liệu tự nhiên, tái tạo nhanh chóng, không đòi hỏi việc chặt phá rừng quy mô lớn như gỗ cứng, giúp bảo vệ hệ sinh thái. Quá trình sản xuất mây tre đan chủ yếu sử dụng lao động thủ công, hạn chế tiêu thụ năng lượng và không tạo ra chất thải công nghiệp độc hại, khác biệt hoàn toàn với các ngành sản xuất hiện đại. Các sản phẩm mây tre đan có độ bền cao, có thể tái sử dụng hoặc tái chế, giảm thiểu rác thải nhựa và các vật liệu không phân hủy. Ngoài ra, việc phát triển nghề mây tre đan còn khuyến khích người dân trồng và bảo vệ cây tre, mây, tạo ra một chu trình sản xuất xanh. Với những ưu điểm này, mây tre đan không chỉ là di sản văn hóa mà còn là một mô hình kinh tế bền vững, được các tổ chức quốc tế và người tiêu dùng hiện đại ngày càng ưa chuộng."
            }
        };


        public void OnGet(int id)
        {
            Storys = Stories.FirstOrDefault(s => s.Id == id);
        }


        public class Story
        {
            public int Id { get; set; }
            public string Title { get; set; }
            public string ImageUrl { get; set; }
            public string Description { get; set; }
        }
    }
}
