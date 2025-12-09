namespace ECommerce.API.Models
{
    public class DiscountCategory
    {
        public Guid DiscountId { get; set; }
        public Guid CategoryId { get; set; }

        public Discount Discount { get; set; } = null!;
        public Category Category { get; set; } = null!;
    }
}
