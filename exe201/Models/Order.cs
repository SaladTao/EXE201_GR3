using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace exe201.Models
{
    public class Order
    {
        public int Id { get; set; }

        public int UserId { get; set; }
        [NotMapped]
        public DateTime OrderDate { get; set; } = DateTime.Now;

        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalAmount { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [Required]

        [EnumDataType(typeof(OrderStatus))]


        public string Status { get; set; } = OrderStatus.Pending.ToString();

        // Quan hệ
        public User User { get; set; }

        public ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
    }

    public enum OrderStatus
    {
        Pending,
        Completed,
        Shipped,
        Cancelled
    }
}
