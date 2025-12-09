using System.ComponentModel.DataAnnotations;

namespace ECommerce.API.Models
{
    public class Inventory
    {
        public Guid Id { get; set; }
        public Guid ProductId { get; set; }
        public Guid? ProductVariantId { get; set; }
        public int Quantity { get; set; }
        public int LowStockThreshold { get; set; } = 10;
        public DateTime LastRestocked { get; set; } = DateTime.UtcNow;
        [MaxLength(100)]
        public string? Location { get; set; }

        public Product Product { get; set; } = null!;
        public ProductVariant? ProductVariant { get; set; }
        public ICollection<InventoryLog> Logs { get; set; } = new List<InventoryLog>();
    }
}
