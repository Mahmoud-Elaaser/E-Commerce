using System.ComponentModel.DataAnnotations;

namespace ECommerce.API.Models
{
    public class Product
    {
        public Guid Id { get; set; }
        [Required, MaxLength(200)]
        public string Name { get; set; } = string.Empty;
        [MaxLength(2000)]
        public string? Description { get; set; }
        [Required, MaxLength(50)]
        public string SKU { get; set; } = string.Empty;
        [Required]
        public decimal Price { get; set; }
        public decimal? DiscountedPrice { get; set; }
        public int QuantityInStock { get; set; }
        public Guid CategoryId { get; set; }
        public Guid? BrandId { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public DateTime LastModified { get; set; } = DateTime.UtcNow;
        public decimal? Weight { get; set; }
        [MaxLength(100)]
        public string? Dimensions { get; set; }

        public Category Category { get; set; } = null!;
        public Brand? Brand { get; set; }
        public ICollection<ProductVariant> Variants { get; set; } = new List<ProductVariant>();
        public ICollection<ProductImage> Images { get; set; } = new List<ProductImage>();
        public ICollection<ProductTag> Tags { get; set; } = new List<ProductTag>();
        public ICollection<ProductReview> Reviews { get; set; } = new List<ProductReview>();
        public Inventory? Inventory { get; set; }
    }
}
