using ECommerce.API.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.API.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<Product> Products { get; set; }
        public DbSet<ProductVariant> ProductVariants { get; set; }
        public DbSet<ProductImage> ProductImages { get; set; }
        public DbSet<ProductTag> ProductTags { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Brand> Brands { get; set; }
        public DbSet<Inventory> Inventories { get; set; }
        public DbSet<InventoryLog> InventoryLogs { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<OrderStatusHistory> OrderStatusHistories { get; set; }
        public DbSet<Address> Addresses { get; set; }
        public DbSet<Discount> Discounts { get; set; }
        public DbSet<DiscountCategory> DiscountCategories { get; set; }
        public DbSet<DiscountProduct> DiscountProducts { get; set; }
        public DbSet<ProductReview> ProductReviews { get; set; }
        public DbSet<ReviewVote> ReviewVotes { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<ShippingMethod> ShippingMethods { get; set; }
        public DbSet<Shipment> Shipments { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Product Configuration
            builder.Entity<Product>(entity =>
            {
                entity.HasIndex(e => e.SKU).IsUnique();
                entity.Property(e => e.Price).HasPrecision(18, 2);
                entity.Property(e => e.DiscountedPrice).HasPrecision(18, 2);

                entity.HasOne(e => e.Category)
                    .WithMany(c => c.Products)
                    .HasForeignKey(e => e.CategoryId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.Brand)
                    .WithMany(b => b.Products)
                    .HasForeignKey(e => e.BrandId)
                    .OnDelete(DeleteBehavior.SetNull);
            });

            // Category Self-referencing
            builder.Entity<Category>(entity =>
            {
                entity.HasOne(e => e.ParentCategory)
                    .WithMany(c => c.SubCategories)
                    .HasForeignKey(e => e.ParentCategoryId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Inventory Configuration
            builder.Entity<Inventory>(entity =>
            {
                entity.HasOne(e => e.Product)
                    .WithOne(p => p.Inventory)
                    .HasForeignKey<Inventory>(e => e.ProductId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Order Configuration
            builder.Entity<Order>(entity =>
            {
                entity.HasIndex(e => e.OrderNumber).IsUnique();
                entity.Property(e => e.Subtotal).HasPrecision(18, 2);
                entity.Property(e => e.TaxAmount).HasPrecision(18, 2);
                entity.Property(e => e.ShippingCost).HasPrecision(18, 2);
                entity.Property(e => e.DiscountAmount).HasPrecision(18, 2);
                entity.Property(e => e.Total).HasPrecision(18, 2);

                entity.HasOne(e => e.User)
                    .WithMany(u => u.Orders)
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // OrderItem Configuration
            builder.Entity<OrderItem>(entity =>
            {
                entity.Property(e => e.UnitPrice).HasPrecision(18, 2);
                entity.Property(e => e.DiscountAmount).HasPrecision(18, 2);
                entity.Property(e => e.TotalPrice).HasPrecision(18, 2);
            });

            // Discount Configuration
            builder.Entity<Discount>(entity =>
            {
                entity.HasIndex(e => e.Code).IsUnique();
                entity.Property(e => e.Value).HasPrecision(18, 2);
                entity.Property(e => e.MinimumOrderAmount).HasPrecision(18, 2);
                entity.Property(e => e.MaximumDiscountAmount).HasPrecision(18, 2);
            });

            // Junction Tables
            builder.Entity<DiscountCategory>(entity =>
            {
                entity.HasKey(e => new { e.DiscountId, e.CategoryId });

                entity.HasOne(e => e.Discount)
                    .WithMany(d => d.DiscountCategories)
                    .HasForeignKey(e => e.DiscountId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.Category)
                    .WithMany()
                    .HasForeignKey(e => e.CategoryId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            builder.Entity<DiscountProduct>(entity =>
            {
                entity.HasKey(e => new { e.DiscountId, e.ProductId });

                entity.HasOne(e => e.Discount)
                    .WithMany(d => d.DiscountProducts)
                    .HasForeignKey(e => e.DiscountId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.Product)
                    .WithMany()
                    .HasForeignKey(e => e.ProductId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Review Configuration
            builder.Entity<ProductReview>(entity =>
            {
                entity.HasOne(e => e.Product)
                    .WithMany(p => p.Reviews)
                    .HasForeignKey(e => e.ProductId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.User)
                    .WithMany(u => u.Reviews)
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Payment Configuration
            builder.Entity<Payment>(entity =>
            {
                entity.Property(e => e.Amount).HasPrecision(18, 2);

                entity.HasOne(e => e.Order)
                    .WithOne(o => o.Payment)
                    .HasForeignKey<Payment>(e => e.OrderId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Shipment Configuration
            builder.Entity<Shipment>(entity =>
            {
                entity.HasOne(e => e.Order)
                    .WithOne(o => o.Shipment)
                    .HasForeignKey<Shipment>(e => e.OrderId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // ShippingMethod Configuration
            builder.Entity<ShippingMethod>(entity =>
            {
                entity.Property(e => e.Cost).HasPrecision(18, 2);
            });
        }
    }
}
