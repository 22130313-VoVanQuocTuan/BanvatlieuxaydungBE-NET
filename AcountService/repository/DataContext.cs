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
        public DbSet<RefreshTokens> RefreshTokens { get; set; }
        public DbSet<InvalidatedToken> InvalidatedTokens { get; set; }
        public DbSet<EmailVerificationCode> EmailVerificationCodes { get; set; }

        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<Shipment> Shipments { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<Cart> Carts { get; set; }

        public DbSet<CartProduct> CartProducts { get; set; }
     

        public DbSet<InfoUserOrder> InfoUserOrders { get; set; }
        public DbSet<Discount> Discount { get; set; }



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


            modelBuilder.Entity<Payment>()
                .HasOne(p => p.Order)
                .WithOne(o => o.Payment)
                .HasForeignKey<Payment>(p => p.OrderId);

            modelBuilder.Entity<Shipment>()
                .HasOne(s => s.Order)
                .WithOne(o => o.Shipment)
                .HasForeignKey<Shipment>(s => s.OrderId);

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
             .HasOne(cp => cp.Order)
             .WithOne(p => p.InfoUserOrder)
             .HasForeignKey<InfoUserOrder>(cp => cp.OrderId);

            modelBuilder.Entity<OrderDetail>()
            .HasOne(cp => cp.Order)
            .WithMany(p => p.OrderDetails)
            .HasForeignKey(cp => cp.OrderId);


            modelBuilder.Entity<EmailVerificationCode>()
       .HasOne(cp => cp.User)
       .WithOne(p => p.EmailVerificationCode)
       .HasForeignKey<EmailVerificationCode>(cp => cp.UserId);









        }
    }
}
