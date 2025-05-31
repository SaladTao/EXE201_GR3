using System.ComponentModel.DataAnnotations;

namespace exe201.Models
{
    public class User
    {
        public int Id { get; set; }

        [Required, MaxLength(100)]
        public string Username { get; set; }

        [Required, MaxLength(255)]
        public string Email { get; set; }

        [Required, MaxLength(255)]
        public string Password { get; set; }

        [Required]
        [EnumDataType(typeof(UserRole))]
        public string Role { get; set; } = UserRole.Customer.ToString();

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public UserProfile Profile { get; set; }
        public ICollection<Cart> Carts { get; set; }
        public ICollection<Order> Orders { get; set; }
        public ICollection<ProductComment> Comments { get; set; }
        public ICollection<ChatMessage> ChatMessages { get; set; }
    }

    public enum UserRole
    {
        Customer,
        Admin
    }

}
