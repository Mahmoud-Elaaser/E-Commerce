namespace ECommerce.DTOs.Order
{
    public record OrderRequest(string BasketId, AddressDto ShipToAddress, int DeliveryMethodId);
}
