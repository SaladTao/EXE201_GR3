namespace exe201.Models
{
    public class ProductComment
    {
        public int Id { get; set; }

        public int ProductId { get; set; }

        public int UserId { get; set; }

        public string Content { get; set; }

        public int Rate { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public Product Product { get; set; }

        public User User { get; set; }
    }

}
