namespace ECommerce.DTOs.Basket
{
    public record BasketDTO
    {
        public string Id { get; init; } = null!;
        public IEnumerable<BasketItemDTO> Items { get; init; } = [];
        public string? ClientSecret { get; set; }
        public string? PaymentIntentId { get; set; }
        public int? DeliveryMethodId { get; set; }
        public decimal? ShippingPrice { get; set; }
    }
}
