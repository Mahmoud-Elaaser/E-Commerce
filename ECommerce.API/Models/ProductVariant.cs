using System.ComponentModel.DataAnnotations;

namespace ECommerce.API.Models
{
    public class ProductVariant
    {
        public Guid Id { get; set; }
        public Guid ProductId { get; set; }
        [MaxLength(50)]
        public string? Color { get; set; }
        [MaxLength(50)]
        public string? Size { get; set; }
        [MaxLength(50)]
        public string? Material { get; set; }
        public decimal AdditionalPrice { get; set; }
        public int Quantity { get; set; }
        [MaxLength(20)]
        public string? SKUSuffix { get; set; }

        public Product Product { get; set; } = null!;
    }
}
