namespace ECommerce.API.Models.Basket
{
    public class CustomerBasket
    {
        public string Id { get; set; } = string.Empty;
        public List<BasketItem> Items { get; set; } = new();
        public string? DiscountCode { get; set; }
        public decimal DiscountAmount { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime LastModified { get; set; } = DateTime.UtcNow;

        public decimal GetSubtotal()
        {
            return Items.Sum(item => item.Price * item.Quantity);
        }

        public decimal GetTotal()
        {
            return GetSubtotal() - DiscountAmount;
        }
    }
}
