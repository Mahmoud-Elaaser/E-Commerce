namespace ECommerce.API.Models
{
    public class DiscountProduct
    {
        public Guid DiscountId { get; set; }
        public Guid ProductId { get; set; }

        public Discount Discount { get; set; } = null!;
        public Product Product { get; set; } = null!;
    }
}
