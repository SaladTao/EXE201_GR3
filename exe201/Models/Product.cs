using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace exe201.Models
{
    public class Product
    {
        public int Id { get; set; }

        [Required, MaxLength(200)]
        public string Name { get; set; }

        public string Description { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }

        [MaxLength(500)]
        public string ImageUrl { get; set; }

        [MaxLength(500)]
        public string Camera360Url { get; set; }

        public bool VirtualGift { get; set; } = false;

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public int CategoryId { get; set; }

        public Category Category { get; set; }

        public ICollection<CartItem> CartItems { get; set; }
        public ICollection<OrderDetail> OrderDetails { get; set; }
        public ICollection<ProductComment> Comments { get; set; }
    }

}
