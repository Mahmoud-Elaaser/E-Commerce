namespace ECommerce.API.Models.Basket
{
    public class BasketItem
    {
        public Guid ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public Guid? ProductVariantId { get; set; }
        public string? VariantDescription { get; set; } // e.g., "Red, Large"
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public string? ImageUrl { get; set; }
        public int MaxQuantity { get; set; } // Available stock
    }
}
