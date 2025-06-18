using Microsoft.EntityFrameworkCore;

namespace exe201.Models
{
    public class EcommerceContext : DbContext
    {
        public EcommerceContext(DbContextOptions<EcommerceContext> options)
            : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<UserProfile> UserProfiles { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<CartItem> CartItems { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<Size> Sizes { get; set; }
        public DbSet<ProductComment> ProductComments { get; set; }
        public DbSet<ChatMessage> ChatMessages { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Username).IsUnique();
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email).IsUnique();
            
            // Add default sizes
            modelBuilder.Entity<Size>().HasData(
                new Size { Id = 1, Name = "Nhỏ" },
                new Size { Id = 2, Name = "Trung Bình" },
                new Size { Id = 3, Name = "Lớn" }
            );
             
            base.OnModelCreating(modelBuilder);
        }
    }
}
