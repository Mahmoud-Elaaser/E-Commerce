using ECommerce.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ECommerce.Configurations
{
    public class DeliveryMethodConfigurations : IEntityTypeConfiguration<DeliveryMethod>
    {
        public void Configure(EntityTypeBuilder<DeliveryMethod> builder)
        {
            builder.Property(d => d.Cost).HasColumnType("decimal(18,2)");
            builder.HasData(
                new DeliveryMethod { Id = 1, ShortName = "Express", Description = "Fast delivery with priority handling", DeliveryTime = "2-3 business days", Cost = 15 },
                new DeliveryMethod { Id = 2, ShortName = "Next Day", Description = "Guaranteed next business day delivery", DeliveryTime = "1 business day", Cost = 25 },
                new DeliveryMethod { Id = 3, ShortName = "Standard", Description = "Economy shipping with tracking", DeliveryTime = "5-7 business days", Cost = 5 },
                new DeliveryMethod { Id = 4, ShortName = "Free Shipping", Description = "Free shipping on all orders", DeliveryTime = "5-10 business days", Cost = 0 },
                new DeliveryMethod { Id = 5, ShortName = "Store Pickup", Description = "Pick up from nearest store location", DeliveryTime = "Ready in 2 hours", Cost = 10 }
            );
        }
    }
}
