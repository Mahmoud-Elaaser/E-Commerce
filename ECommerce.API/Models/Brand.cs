using System.ComponentModel.DataAnnotations;

namespace ECommerce.API.Models
{
    public class Brand
    {
        public Guid Id { get; set; }
        [Required, MaxLength(100)]
        public string Name { get; set; } = string.Empty;
        [MaxLength(500)]
        public string? Description { get; set; }
        [MaxLength(500)]
        public string? LogoUrl { get; set; }
        [MaxLength(200)]
        public string? WebsiteUrl { get; set; }
        public bool IsActive { get; set; } = true;

        public ICollection<Product> Products { get; set; } = new List<Product>();
    }
}
