using System.ComponentModel.DataAnnotations;

namespace ECommerce.API.Models
{
    public class ProductTag
    {
        public Guid Id { get; set; }
        public Guid ProductId { get; set; }
        [Required, MaxLength(50)]
        public string Tag { get; set; } = string.Empty;

        public Product Product { get; set; } = null!;
    }
}
