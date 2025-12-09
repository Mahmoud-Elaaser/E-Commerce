using ECommerce.API.Enums;
using System.ComponentModel.DataAnnotations;

namespace ECommerce.API.Models
{
    public class Discount
    {
        public Guid Id { get; set; }
        [Required, MaxLength(50)]
        public string Code { get; set; } = string.Empty;
        [MaxLength(500)]
        public string? Description { get; set; }
        public DiscountType DiscountType { get; set; }
        public decimal Value { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal? MinimumOrderAmount { get; set; }
        public decimal? MaximumDiscountAmount { get; set; }
        public bool IsActive { get; set; } = true;
        public int? UseLimit { get; set; }
        public int UsedCount { get; set; }

        public ICollection<DiscountCategory> DiscountCategories { get; set; } = new List<DiscountCategory>();
        public ICollection<DiscountProduct> DiscountProducts { get; set; } = new List<DiscountProduct>();
    }
}
