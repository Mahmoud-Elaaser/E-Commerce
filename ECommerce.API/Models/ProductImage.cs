using System.ComponentModel.DataAnnotations;

namespace ECommerce.API.Models
{
    public class ProductImage
    {
        public Guid Id { get; set; }
        public Guid ProductId { get; set; }
        [Required, MaxLength(500)]
        public string ImageUrl { get; set; } = string.Empty;
        public int DisplayOrder { get; set; }
        public bool IsPrimary { get; set; }

        public Product Product { get; set; } = null!;
    }
}
