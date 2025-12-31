namespace ECommerce.DTOs.Basket
{
    public record CustomerBasketDto(
        string Id,
        string? ClientSecret,
        string? PaymentIntentId,
        int? DeliveryMethodId,
        IEnumerable<BasketItemDTO> Items
    );
}
