namespace ECommerce.DTOs.Basket
{
    public class BasketDTO
    {
        public string Id { get; init; } = null!;
        public string? ClientSecret { get; set; }
        public string? PaymentIntentId { get; set; }
        public int? DeliveryMethodId { get; set; }
        public decimal? ShippingPrice { get; set; }
        public IEnumerable<BasketItemDTO> Items { get; set; } = new List<BasketItemDTO>();
    }
}
