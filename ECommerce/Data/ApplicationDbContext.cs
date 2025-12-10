using ECommerce.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>

    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<Product> Products { get; set; }
        public DbSet<ProductBrand> ProductBrands { get; set; }
        public DbSet<ProductType> ProductTypes { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<DeliveryMethod> DeliveryMethods { get; set; }
        public DbSet<Address> Addresses { get; set; }

        override protected void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<DeliveryMethod>()
                .Property(d => d.Cost)
                .HasColumnType("decimal(18,2)");


            modelBuilder.Entity<OrderItem>().OwnsOne(oi => oi.Product, p => p.WithOwner());
            modelBuilder.Entity<OrderItem>()
                .Property(oi => oi.Price)
                 .HasColumnType("decimal(18,2)");



            modelBuilder.Entity<Product>().HasOne(p => p.ProductBrand)
                   .WithMany()
                   .HasForeignKey(p => p.ProductBrandId);
            modelBuilder.Entity<Product>().HasOne(p => p.ProductType)
                   .WithMany()
                   .HasForeignKey(p => p.ProductTypeId);
            modelBuilder.Entity<Product>().Property(p => p.Price)
                   .HasColumnType("decimal(18,2)");


            modelBuilder.Entity<Order>().HasMany(o => o.OrderItems)
                   .WithOne()
                   .OnDelete(DeleteBehavior.Cascade);


            modelBuilder.Entity<Order>().HasOne(o => o.DeliveryMethod)
                   .WithMany()
                   .HasForeignKey(o => o.DeliveryMethodId)
                   .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Order>().Property(o => o.Subtotal)
                   .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<Address>().ToTable("Adresses");

        }
    }
}
