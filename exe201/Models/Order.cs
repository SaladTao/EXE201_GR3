using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace exe201.Models
{
    public class Order
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalAmount { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [Required]
        [EnumDataType(typeof(UserRole))]
        public string Status { get; set; } = OrderStatus.Pending.ToString();

        public User User { get; set; }

        public ICollection<OrderDetail> OrderDetails { get; set; }
    }
    public enum OrderStatus
    {
        Pending,
        Completed,
        Shipping,
        Cancelled
    }
}
