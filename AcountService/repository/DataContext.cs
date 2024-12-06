using AcountService.entity;
using BanVatLieuXayDung.entity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AcountService.Repository
{
    public class DataContext : IdentityDbContext<User>
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Password_reset> password_Resets { get; set; }
        public DbSet<RefreshTokens> RefreshTokens { get; set; }
        public DbSet<InvalidatedToken> InvalidatedTokens { get; set; }
        public DbSet<EmailVerificationCode> EmailVerificationCodes { get; set; }

        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }
     //   public DbSet<Payment> Payments { get; set; }
    //    public DbSet<Shipment> Shipments { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<Cart> Carts { get; set; }

        public DbSet<CartProduct> CartProducts { get; set; }
     

        public DbSet<InfoUserOrder> InfoUserOrders { get; set; }
        public DbSet<Discount> Discount { get; set; }
        public DbSet<PromotionalProducts> PromotionalProducts { get; set; }



        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            // Configure relationships
            modelBuilder.Entity<Product>()
            .HasOne(p => p.Category)
            .WithMany(c => c.Products)
            .HasForeignKey(p => p.CategoryId)
            .OnDelete(DeleteBehavior.Cascade);  // Cấu hình Cascade Delete

            modelBuilder.Entity<Order>()
                .HasMany(o => o.OrderDetails)
                .WithOne(od => od.Order)
                .HasForeignKey(odf => odf.OrderId);

           modelBuilder.Entity<User>()
             .HasMany(u => u.Orders)  // Cấu hình mối quan hệ từ User đến Orders
             .WithOne(o => o.User)    // Mỗi Order thuộc về một User
             .HasForeignKey(o => o.UserId);  // Khóa ngoại trong Order trỏ đến User


            modelBuilder.Entity<Review>()
                .HasOne(r => r.Product)
                .WithMany(p => p.Reviews)
                .HasForeignKey(r => r.ProductId);

            modelBuilder.Entity<Review>()
                .HasOne(r => r.User)
                .WithMany(u => u.Reviews)
                .HasForeignKey(r => r.UserId);

            modelBuilder.Entity<Cart>()
                .HasOne(c => c.User)
                .WithOne(u => u.Cart)
                .HasForeignKey<Cart>(c => c.UserId);


            modelBuilder.Entity<Cart>()
               .HasMany(c => c.CartProducts)
               .WithOne(u => u.Cart)
               .HasForeignKey(c => c.CartId);




            modelBuilder.Entity<CartProduct>()
                .HasOne(cp => cp.Cart)
                .WithMany(c => c.CartProducts)
                .HasForeignKey(cp => cp.CartId);

            modelBuilder.Entity<CartProduct>()
                .HasOne(cp => cp.Product)
                .WithMany(p => p.CartProducts)
                .HasForeignKey(cp => cp.ProductId);



            modelBuilder.Entity<InfoUserOrder>()
             .HasOne(cp => cp.Users)
             .WithMany(p => p.infoUserOrders)
             .HasForeignKey(cp => cp.UserId);

          

          

            modelBuilder.Entity<OrderDetail>()
            .HasOne(cp => cp.Order)
            .WithMany(p => p.OrderDetails)
            .HasForeignKey(cp => cp.OrderId);


            modelBuilder.Entity<EmailVerificationCode>()
           .HasOne(cp => cp.User)
           .WithOne(p => p.EmailVerificationCode)
           .HasForeignKey<EmailVerificationCode>(cp => cp.UserId);

            modelBuilder.Entity<PromotionalProducts>()
            .HasOne(cp => cp.Product)
            .WithMany(p => p.PromotionalProducts)
            .HasForeignKey(cp => cp.ProductId);

            modelBuilder.Entity<Password_reset>()
            .HasOne(cp => cp.User)
            .WithMany(p => p.password_Resets)
            .HasForeignKey(cp => cp.UserId);


            modelBuilder.Entity<PromotionalProducts>()
            .HasOne(cp => cp.Product)
            .WithMany(p => p.PromotionalProducts)
            .HasForeignKey(cp => cp.ProductId);

            modelBuilder.Entity<PromotionalProducts>()
       .Property(p => p.DiscountPercentage)
       .HasPrecision(5, 2); // 5 chữ số tổng cộng, 2 chữ số thập phân

            // chuyển về dạng decimal
            modelBuilder.Entity<CartProduct>()
      .Property(p => p.Price)
      .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<CartProduct>()
                .Property(p => p.TotalPrice)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<CartProduct>()
            .Property(p => p.discount_amount)
            .HasColumnType("decimal(10,2)");

            modelBuilder.Entity<Discount>()
                .Property(p => p.Percent)

                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<Order>()
                .Property(p => p.TotalPrice)
                .HasColumnType("decimal(18,2)");
            modelBuilder.Entity<Order>()
                .Property(p => p.shipping_fee)
                .HasColumnType("decimal(10,2)");
            modelBuilder.Entity<Order>()
                .Property(p => p.discount_amount)
                .HasColumnType("decimal(10,2)");

            modelBuilder.Entity<OrderDetail>()
                .Property(p => p.TotalPrice)
                .HasColumnType("decimal(18,2)");
            modelBuilder.Entity<OrderDetail>()
                .Property(od => od.Price)
                .HasColumnType("decimal(18,2)");
            modelBuilder.Entity<OrderDetail>()
                .Property(od => od.Discount)
               .HasColumnType("decimal(10,2)");

            modelBuilder.Entity<Payment>()
                .Property(p => p.Amount)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<Product>()
                .Property(p => p.Price)
                .HasColumnType("decimal(18,2)");
        
    

            modelBuilder.Entity<Cart>()
                .Property(p => p.TotalPrice)
                .HasColumnType("decimal(18,2)");
            modelBuilder.Entity<Cart>()
              .Property(p => p.discount_amount)
              .HasColumnType("decimal(10,2)");
            modelBuilder.Entity<Cart>()
              .Property(p => p.shipping_fee)
              .HasColumnType("decimal(10,2)");
            modelBuilder.Entity<Cart>()
             .Property(p => p.code_discount)
             .HasColumnType("decimal(10,2)");
            modelBuilder.Entity<Cart>()
             .Property(p => p.promotion_discount)
             .HasColumnType("decimal(10,2)");



        }
    }
}
