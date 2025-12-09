using System.ComponentModel.DataAnnotations;

namespace ECommerce.API.Models
{
    public class ShippingMethod
    {
        public Guid Id { get; set; }
        [Required, MaxLength(100)]
        public string Name { get; set; } = string.Empty;
        [MaxLength(500)]
        public string? Description { get; set; }
        public decimal Cost { get; set; }
        [MaxLength(100)]
        public string? DeliveryTime { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
