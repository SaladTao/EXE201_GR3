using Microsoft.AspNetCore.Mvc;

namespace exe201.Models
{
    public class Story
    {
        public int Id { get; set; }                    
        public string Title { get; set; }              
        public string ImageUrl { get; set; }          
        public string Description { get; set; }        
        public DateTime PostedDate { get; set; }       
        public string Category { get; set; }

    }

    public static class StoryRepository
    {
        public static List<Story> Stories = new List<Story>
{
    new Story { Id = 1, Title = "Câu truyện về đèn lồng mây tre đan", ImageUrl = "https://maytretrungphuong.com/wp-content/uploads/2021/09/Den-hat-cuom-ruc-ro-dep-mat.png", Description = "", PostedDate = new DateTime(2025, 1, 10), Category = "Trang trí nội thất" },
    new Story { Id = 2, Title = "Câu truyện về người dân làng nghề mây tre đan", ImageUrl = "https://file3.qdnd.vn/data/images/0/2022/10/27/vuhuyen/may-tre.jpg?dpi=150&quality=100&w=870", Description = "", PostedDate = new DateTime(2025, 2, 5), Category = "Văn hóa dân gian" },
    new Story { Id = 3, Title = "Nghệ thuật mây tre đan qua từng thế hệ", ImageUrl = "https://nguoinamdinh.net/wp-content/uploads/2016/06/1-128.jpg", Description = "", PostedDate = new DateTime(2025, 3, 1), Category = "Di sản văn hóa" },
    new Story { Id = 4, Title = "Hành trình về sản phẩm tinh xảo", ImageUrl = "https://dntt.mediacdn.vn/197608888129458176/2023/12/16/lang-may-tre-dan-phu-vinh-11675520593-1702739108477352274759.jpeg", Description = "", PostedDate = new DateTime(2025, 4, 18), Category = "Nghề thủ công" },
    new Story { Id = 5, Title = "Mây tre đan trong đời sống hàng ngày", ImageUrl = "https://dentrangtrimaianh.com/wp-content/uploads/2023/11/Diem-danh-15-lang-nghe-may-tre-dan-truyen-thong-noi-tieng-khap-ca-nuoc-Noi-tao-nen-nhung-mon-do-nghe-thuat-va-mang-lai-nguon-thu-nhap-dang-ke-cho-nguoi-dan-dia-phuong.jpg", Description = "", PostedDate = new DateTime(2025, 5, 2), Category = "Đời sống" },
    new Story { Id = 6, Title = "Sự đổi mới trong nghề mây tre đan", ImageUrl = "https://encrypted-tbn0.gstatic.com/images?q=tb&n:ANd9GcR73am97KcUjDZpOM09W1T0MBlPMauz9WscaA&s", Description = "", PostedDate = new DateTime(2025, 6, 1), Category = "Thiết kế & Đổi mới" },
    new Story { Id = 7, Title = "Mây tre đan và môi trường bền vững", ImageUrl = "https://vungdecor.com/wp-content/uploads/2024/12/do-may-tre-dan-hcm2.png", Description = "", PostedDate = new DateTime(2025, 6, 5), Category = "Sống xanh" },
    new Story { Id = 8, Title = "Phú Vinh – Nơi mây tre kể chuyện nghìn năm 🌾", ImageUrl = "", Description = "", PostedDate = new DateTime(2025, 6, 6), Category = "Làng nghề truyền thống" },
    new Story { Id = 9, Title = "Tại sao nên chọn sản phẩm thân thiện môi trường?", ImageUrl = "", Description = "", PostedDate = new DateTime(2025, 6, 7), Category = "Ý thức môi trường" },
    new Story { Id = 10, Title = "Cùng Ecoloom decor cho không gian xanh", ImageUrl = "", Description = "", PostedDate = new DateTime(2025, 6, 8), Category = "Trang trí nội thất" },
    new Story { Id = 11, Title = "Top 5 vật dụng tre giúp decor nhà xinh", ImageUrl = "", Description = "", PostedDate = new DateTime(2025, 6, 9), Category = "Gợi ý trang trí" },
    new Story { Id = 12, Title = "1 ngày của nghệ nhân đan mây", ImageUrl = "", Description = "", PostedDate = new DateTime(2025, 6, 10), Category = "Câu chuyện làng nghề" },
    new Story { Id = 13, Title = "Câu chuyện khách hàng – Sống xanh bắt đầu từ điều nhỏ nhất", ImageUrl = "", Description = "", PostedDate = new DateTime(2025, 6, 11), Category = "Trải nghiệm khách hàng" },
    new Story { Id = 14, Title = "Mẹo bảo quản sản phẩm tre", ImageUrl = "", Description = "", PostedDate = new DateTime(2025, 6, 12), Category = "Hướng dẫn sử dụng" },
    new Story { Id = 15, Title = "Quote hay về sống xanh, tối giản", ImageUrl = "", Description = "", PostedDate = new DateTime(2025, 6, 13), Category = "Truyền cảm hứng" },
    new Story { Id = 16, Title = "Mỗi căn nhà – Một bản sắc sống xanh riêng biệt", ImageUrl = "", Description = "", PostedDate = new DateTime(2025, 6, 14), Category = "Phong cách sống" },
    new Story { Id = 17, Title = "Bạn không cần phải cầu kỳ để nổi bật – chỉ cần một chiếc túi mây xinh thủ công là đủ.", ImageUrl = "", Description = "", PostedDate = new DateTime(2025, 6, 15), Category = "Phong cách cá nhân" },
    new Story { Id = 18, Title = "Đồ gia dụng tre – sống xanh từ bếp", ImageUrl = "", Description = "", PostedDate = new DateTime(2025, 6, 16), Category = "Gia dụng" },
    new Story { Id = 19, Title = "Túi mây thủ công – mỗi chiếc là một tác phẩm", ImageUrl = "", Description = "", PostedDate = new DateTime(2025, 6, 17), Category = "Thời trang thủ công" },
    new Story { Id = 20, Title = "Trang sức mây – Nét chấm phá cho nàng nhẹ nhàng", ImageUrl = "", Description = "", PostedDate = new DateTime(2025, 6, 18), Category = "Trang sức" },
    new Story { Id = 21, Title = "Tủ đồ “Xanh” cho cô nàng yêu thiên nhiên", ImageUrl = "", Description = "", PostedDate = new DateTime(2025, 6, 19), Category = "Thời trang bền vững" },
    new Story { Id = 22, Title = "Bộ đôi hoàn hảo: Túi xách + phụ kiện mây", ImageUrl = "", Description = "", PostedDate = new DateTime(2025, 6, 20), Category = "Phối đồ" },
    new Story { Id = 23, Title = "Review từ khách hàng thật", ImageUrl = "", Description = "", PostedDate = new DateTime(2025, 6, 21), Category = "Khách hàng & Cộng đồng" }
};


        public static List<Story> GetAll() => Stories;
         
        public static Story GetById(int id) => Stories.FirstOrDefault(s => s.Id == id);
    }

}
