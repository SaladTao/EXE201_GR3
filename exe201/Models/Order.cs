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

        public DateTime OrderDate { get; set; } = DateTime.Now;

        //Add Name, Address and Phone
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public decimal Balance { get; set; }
         
        public string Phone { get; set; }
        public string Address { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalAmount { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [Required]
        public string Status { get; set; } = OrderStatus.Pending.ToString();

        // Quan hệ
        public User User { get; set; }

        public ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
    }

    public enum OrderStatus
    {
        Pending,
        Completed,
        Shipping,
        Cancelled
    }
}
