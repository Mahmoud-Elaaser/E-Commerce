namespace ECommerce.DTOs.Basket
{
    public class BasketResponseDto
    {
        public string Id { get; set; }
        public string? PaymentIntentId { get; set; }
        public string? ClientSecret { get; set; }
        public int DeliveryMethodId { get; set; }
        public List<BasketItemDTO> Items { get; set; }
    }
}
